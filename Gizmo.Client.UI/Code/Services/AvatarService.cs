using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// The signed-in customer's own picture: read on sign-in, replaced from the editor.
    /// Off unless the club asks for it.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why a service of the club's rather than the client's.</b> Gizmo 3.0.95 has no
    /// self-service picture: the user surface of the Web API (<c>api/user/v3/...</c>) has
    /// no route for it, <c>UserProfileModelUpdate</c> carries names and a birth date but
    /// no picture, and <c>GET/PUT api/v3/users/{id}/picture</c> is the management API -
    /// an operator login, which a station must never hold. A club that wants pictures
    /// therefore runs a small service next to its server (<c>deploy\avatar-proxy</c>)
    /// that keeps the operator credentials on the server and offers the station a plain
    /// <c>/avatar/{id}</c>. This class speaks to that service and nothing else; the day
    /// the user surface grows a picture route, only <see cref="RefreshAsync"/> and
    /// <see cref="UploadAsync"/> change.
    /// </para>
    /// <para>
    /// <b>Off by default.</b> Nothing here runs, and no picture is shown or offered,
    /// unless the club's stylesheet says so (see <see cref="Configure"/>). A club that
    /// never heard of this pays nothing for it.
    /// </para>
    /// <para>
    /// Static, like <see cref="Loyalty"/>: the host registers nothing from the skin
    /// assembly, and the state has to outlive any page. <see cref="Attach"/> is called
    /// once from <c>App</c>.
    /// </para>
    /// </remarks>
    public sealed class AvatarService
    {
        /// <summary>The port <c>deploy\avatar-proxy\config.json</c> listens on by default.</summary>
        private const int DefaultPort = 8765;

        /// <summary>
        /// How long after sign-in the invitation waits before it slides in, so that it
        /// does not compete with the shell's own entrance animations.
        /// </summary>
        private static readonly TimeSpan NudgeShowDelay = TimeSpan.FromSeconds(2.5);

        #region STATIC ACCESS

        public static AvatarService Current { get; private set; }

        /// <summary>Whether the club asked for pictures and the service is ready.</summary>
        public static bool IsEnabled => Current is { _enabled: true };

        /// <summary>
        /// Hooks the client's events. Idempotent; the first call wins. Nothing happens
        /// until <see cref="Configure"/> says the club wants pictures.
        /// </summary>
        public static void Attach(IServiceProvider services)
        {
            if (Current is not null || services is null)
                return;

            var client = services.GetService<IGizmoClient>();
            var userViewState = services.GetService<UserViewState>();

            if (client is null || userViewState is null)
                return;

            Current = new AvatarService(client,
                services.GetService<NavigationService>(),
                services.GetService<IOptionsMonitor<ClientNetworkOptions>>(),
                userViewState,
                services.GetService<ILoggerFactory>()?.CreateLogger("Grafit.Avatars"));
        }

        #endregion

        private AvatarService(IGizmoClient gizmoClient,
            NavigationService navigationService,
            IOptionsMonitor<ClientNetworkOptions> networkOptions,
            UserViewState userViewState,
            ILogger logger)
        {
            _gizmoClient = gizmoClient;
            _navigationService = navigationService;
            _networkOptions = networkOptions;
            _userViewState = userViewState;
            _logger = logger;

            // Not through IHttpClientFactory: that means a named registration in the
            // host's container, which the skin stays out of. One owned client for the
            // lifetime of a singleton is the ordinary pattern anyway.
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

            _gizmoClient.LoginStateChange += OnLoginStateChange;

            // LoginStateChange alone misses the shell being reloaded while a customer is
            // already signed in - no login happens, so no event fires. The user view
            // state is filled in both paths and covers the gap.
            _userViewState.OnChange += OnUserViewStateChanged;
        }

        #region FIELDS

        private readonly IGizmoClient _gizmoClient;
        private readonly NavigationService _navigationService;
        private readonly IOptionsMonitor<ClientNetworkOptions> _networkOptions;
        private readonly UserViewState _userViewState;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        private bool _enabled;
        private string _configuredBase;
        private int _userId;

        #endregion

        #region PROPERTIES

        /// <summary>The picture as a data URI, or null: no picture, or the club has this off.</summary>
        public string Picture { get; private set; }

        /// <summary>Whether the invitation to add a picture is on screen.</summary>
        public bool ShowNudge { get; private set; }

        /// <summary>
        /// Why the last operation failed, in the customer's language, or null. Shown in
        /// the editor as it is: "it did not work, try later" leaves an operator with
        /// nothing to go on when the service is simply not running.
        /// </summary>
        public string LastError { get; private set; }

        #endregion

        #region EVENTS

        /// <summary>Raised after <see cref="Picture"/> or <see cref="ShowNudge"/> changes.</summary>
        /// <remarks>
        /// Raised from HTTP continuations and client events, never on the renderer's
        /// thread: subscribers marshal for themselves (see <c>ShellDispatch</c>). One
        /// subscriber throwing must not stop the others.
        /// </remarks>
        public event Action Changed;

        private void RaiseChanged()
        {
            var handlers = Changed;

            if (handlers is null)
                return;

            foreach (var handler in handlers.GetInvocationList())
            {
                try
                {
                    ((Action)handler).Invoke();
                }
                catch (Exception exception)
                {
                    _logger?.LogWarning(exception, "Avatar change subscriber failed.");
                }
            }
        }

        #endregion

        #region CONFIGURATION

        /// <summary>
        /// Applies the club's <c>--gg-avatars</c> key, read from the stylesheet once the
        /// shell has rendered.
        /// </summary>
        /// <param name="value">
        /// <c>on</c> - pictures, with the service on the Gizmo server's own machine;
        /// an address such as <c>http://10.0.0.5:8765</c> - pictures, with the service
        /// there; anything else, including nothing at all - off.
        /// </param>
        /// <returns>true when pictures are on.</returns>
        public bool Configure(string value)
        {
            var wanted = (value ?? string.Empty).Trim().Trim('"', '\'');

            if (wanted.Length == 0 || wanted.Equals("off", StringComparison.OrdinalIgnoreCase))
            {
                _enabled = false;
                _configuredBase = null;
                return false;
            }

            if (wanted.Equals("on", StringComparison.OrdinalIgnoreCase))
            {
                _enabled = true;
                _configuredBase = null;
            }
            else if (Uri.TryCreate(wanted, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                _enabled = true;
                _configuredBase = uri.GetLeftPart(UriPartial.Authority);
            }
            else
            {
                _logger?.LogWarning("--gg-avatars: {Value} is neither on, off nor an address; pictures stay off.", wanted);
                _enabled = false;
                return false;
            }

            // A customer can already be signed in when this arrives: the key is read
            // after the first render, the shell may have been reloaded under them.
            var id = _userViewState.Id;

            if (id > 0 && !_userViewState.IsGuest)
            {
                _userId = id;
                Forget(RefreshAsync(id, default));
            }

            return true;
        }

        #endregion

        #region EVENT HANDLERS

        // The client raises these from its own threads. They are deliberately not
        // `async void`: an exception escaping such a method lands on the thread pool and
        // exits the whole client. A picture is decoration and must never be able to end
        // a customer's session.
        private void OnLoginStateChange(object sender, UserLoginStateChangeEventArgs e)
        {
            if (!_enabled)
                return;

            if (e.State == LoginState.LoginCompleted)
            {
                var profile = e.UserProfile;

                // A guest has no account to carry a picture.
                if (profile is null || profile.IsGuest)
                    return;

                _userId = profile.Id;
                Forget(RefreshAsync(profile.Id, default));
            }
            else if (e.State == LoginState.LoggingOut)
            {
                // Cleared at once: this is a shared machine, and the next customer must
                // never see the previous one's picture while their own loads.
                _userId = 0;
                Picture = null;
                ShowNudge = false;
                RaiseChanged();
            }
        }

        private void OnUserViewStateChanged(object sender, EventArgs e)
        {
            if (!_enabled)
                return;

            var id = _userViewState.Id;

            // Only a move to a different real account: this state raises for every
            // profile field as well.
            if (id <= 0 || id == _userId || _userViewState.IsGuest)
                return;

            _userId = id;
            Forget(RefreshAsync(id, default));
        }

        /// <summary>
        /// Runs a task nobody awaits, and makes sure a fault is logged rather than
        /// unobserved.
        /// </summary>
        private void Forget(Task task)
        {
            if (task is null || task.IsCompletedSuccessfully)
                return;

            task.ContinueWith(faulted => _logger?.LogWarning(faulted.Exception, "Avatar work failed."),
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);
        }

        #endregion

        #region FUNCTIONS

        /// <summary>
        /// The address of the club's picture service.
        /// </summary>
        /// <remarks>
        /// With <c>--gg-avatars: on</c> it is the Gizmo server's own machine on
        /// <see cref="DefaultPort"/>. The server's address is the one the client already
        /// uses for every call (<c>options.json</c> -&gt; <c>Network.ServerUri</c>), not
        /// the page's base URI: inside the WPF host the shell is served by the WebView's
        /// virtual host, so the base URI is <c>https://0.0.0.0/</c> - routable nowhere.
        /// The base URI stays as the fallback for the web host, where the server does
        /// serve the shell and <c>ServerUri</c> is normally empty.
        /// </remarks>
        private string GetBaseUrl()
        {
            if (_configuredBase is not null)
                return _configuredBase;

            var serverUri = _networkOptions?.CurrentValue?.ServerUri;

            if (string.IsNullOrWhiteSpace(serverUri))
                serverUri = _navigationService?.GetBaseUri();

            if (string.IsNullOrWhiteSpace(serverUri) || !Uri.TryCreate(serverUri, UriKind.Absolute, out var uri))
            {
                LastError = ShellStringOverrides.Get(ShellStringOverrides.AVATAR_NO_SERVER);
                return null;
            }

            var host = uri.Host;

            // The WebView's virtual host, if the fallback was taken: on the desktop shell
            // the only sensible reading of it is "this machine".
            if (host is "0.0.0.0" or "[::]")
                host = "localhost";

            return $"http://{host}:{DefaultPort}";
        }

        /// <summary>
        /// Reads the customer's picture. Any failure - service not running, no picture
        /// set, no network - leaves <see cref="Picture"/> unset: a missing picture is
        /// never an error worth showing.
        /// </summary>
        public async Task RefreshAsync(int userId, CancellationToken cancellationToken)
        {
            if (!_enabled || userId <= 0)
                return;

            var baseUrl = GetBaseUrl();

            if (baseUrl is null)
                return;

            try
            {
                using var response = await _httpClient.GetAsync($"{baseUrl}/avatar/{userId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    // 404: this customer has no picture - not an error.
                    Picture = null;
                    RaiseChanged();

                    // Separate, slower-paced concern with a delay of its own; it must
                    // never hold up the sign-in this runs on.
                    Forget(CheckNudgeAsync(userId));
                    return;
                }

                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";

                Picture = $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
                RaiseChanged();
            }
            catch (Exception exception)
            {
                // The service is not running or not reachable. Quiet: the glyph stands.
                _logger?.LogWarning(exception, "Avatar fetch failed for user {UserId}.", userId);
            }
        }

        /// <summary>
        /// Asks the service whether this sign-in should invite the customer to add a
        /// picture - the first one, then every few sessions. A local lookup on its side,
        /// no extra call to Gizmo.
        /// </summary>
        private async Task CheckNudgeAsync(int userId)
        {
            var baseUrl = GetBaseUrl();

            if (baseUrl is null)
                return;

            try
            {
                using var response = await _httpClient.GetAsync($"{baseUrl}/avatar/{userId}/nudge");

                if (!response.IsSuccessStatusCode)
                    return;

                var result = await response.Content.ReadFromJsonAsync<NudgeCheckResult>();

                if (result is null || !result.Show)
                    return;

                await Task.Delay(NudgeShowDelay);

                // The customer may have signed out while this waited.
                if (_userId != userId)
                    return;

                ShowNudge = true;
                RaiseChanged();
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Avatar nudge check failed for user {UserId}.", userId);
            }
        }

        /// <summary>
        /// Hides the invitation - dismissed by the customer, or timed out on screen.
        /// </summary>
        public void DismissNudge()
        {
            if (!ShowNudge)
                return;

            ShowNudge = false;
            RaiseChanged();
        }

        /// <summary>
        /// Stores a new picture for the signed-in customer. The bytes are expected to be
        /// cropped and compressed already (the editor does that in the browser); nothing
        /// is processed here.
        /// </summary>
        public async Task<bool> UploadAsync(byte[] imageBytes, string contentType, CancellationToken cancellationToken)
        {
            LastError = null;

            if (!_enabled)
                return false;

            var baseUrl = GetBaseUrl();

            if (baseUrl is null)
                return false;

            if (_userId <= 0)
            {
                LastError = ShellStringOverrides.Get(ShellStringOverrides.AVATAR_NO_ACCOUNT);
                return false;
            }

            try
            {
                using var content = new ByteArrayContent(imageBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                using var response = await _httpClient.PostAsync($"{baseUrl}/avatar/{_userId}", content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger?.LogWarning("Avatar upload rejected: {Status}", response.StatusCode);
                    LastError = ShellStringOverrides.Get(ShellStringOverrides.AVATAR_REJECTED, (int)response.StatusCode);
                    return false;
                }

                Picture = $"data:{contentType};base64,{Convert.ToBase64String(imageBytes)}";
                RaiseChanged();

                DismissNudge();

                return true;
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Avatar upload failed.");
                LastError = ShellStringOverrides.Get(ShellStringOverrides.AVATAR_NO_SERVICE, baseUrl);
                return false;
            }
        }

        #endregion

        private sealed class NudgeCheckResult
        {
            [JsonPropertyName("show")]
            public bool Show { get; set; }
        }
    }
}
