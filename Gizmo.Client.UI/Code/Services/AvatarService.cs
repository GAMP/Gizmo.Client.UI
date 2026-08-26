using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Client;
using Gizmo.Client.Options;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Fetches/uploads the current user's avatar through the local avatar
    /// proxy (see deploy/avatar-proxy in the repo).
    /// </summary>
    /// <remarks>
    /// Deliberately NOT a DI-registered service, and deliberately kept in
    /// THIS project rather than the Gizmo.Client.UI.Services submodule.
    /// composition.json's AdditionalAssemblies only lists Gizmo.Client.UI.dll
    /// and Gizmo.Web.Components.dll - those are the only two assemblies the
    /// skin loader actually reloads from the skin folder (Assembly.LoadFrom).
    /// Any other project reference is loaded by the host process at its own
    /// startup (Gizmo.Client.UI.Services.dll included - the host needs it
    /// immediately for AddClientServices()), and once an assembly with a
    /// given identity is loaded, Assembly.LoadFrom returns that cached copy
    /// for any later request with the same name - a newer file dropped in
    /// the skin folder is silently ignored. Shipping this class inside
    /// Gizmo.Client.UI.Services.dll caused exactly that: the host's own
    /// (old) copy loaded instead of the skin's, Gizmo.Client.UI.dll
    /// (compiled expecting the new type) threw a TypeLoadException trying
    /// to use it, and the whole shell got stuck on "loading" on the real
    /// server. Keeping this class inside Gizmo.Client.UI.dll itself avoids
    /// the problem entirely - that assembly is never host-resident, so it's
    /// genuinely reloaded fresh from the skin folder every time.
    ///
    /// Because it isn't DI-registered, nothing outside this assembly needs
    /// to resolve it: it's constructed exactly once by App.razor.cs (the
    /// root component, initialized once per app session) and reached from
    /// anywhere else in this assembly through the static Current property.
    /// </remarks>
    public sealed class AvatarService
    {
        // Must match "listen_port" in deploy/avatar-proxy/config.json.
        private const int ProxyPort = 8765;

        // How long the nudge banner waits after login before sliding in -
        // long enough that it doesn't compete with the shell's own
        // entrance animations for attention.
        private static readonly TimeSpan NudgeShowDelay = TimeSpan.FromSeconds(2.5);

        #region STATIC ACCESS

        public static AvatarService? Current { get; private set; }

        /// <summary>
        /// Constructs the singleton instance. Safe to call more than once
        /// (e.g. if App.razor.cs re-initializes) - only the first call
        /// takes effect.
        /// </summary>
        public static AvatarService Initialize(IGizmoClient gizmoClient,
            NavigationService navigationService,
            IOptionsMonitor<ClientNetworkOptions> networkOptions,
            UserViewState userViewState,
            ILogger<AvatarService> logger)
        {
            Current ??= new AvatarService(gizmoClient, navigationService, networkOptions, userViewState, logger);
            return Current;
        }

        #endregion

        #region CONSTRUCTOR

        private AvatarService(IGizmoClient gizmoClient,
            NavigationService navigationService,
            IOptionsMonitor<ClientNetworkOptions> networkOptions,
            UserViewState userViewState,
            ILogger<AvatarService> logger)
        {
            _gizmoClient = gizmoClient;
            _navigationService = navigationService;
            _networkOptions = networkOptions;
            _userViewState = userViewState;
            _logger = logger;

            // Not going through IHttpClientFactory - that requires a named
            // registration through the same frozen host DI container this
            // class is deliberately staying out of. A single owned
            // HttpClient for this service's whole lifetime is the normal
            // pattern for a long-lived singleton like this one anyway.
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };

            _gizmoClient.LoginStateChange += OnLoginStateChange;

            // LoginStateChange alone misses the case where the shell itself
            // is (re)loaded while a user is already logged in - no login
            // happens, so no event fires, and the avatar never loads. The
            // user view state is populated in both paths, so watching it
            // covers the gap.
            _userViewState.OnChange += OnUserViewStateChanged;
        }

        #endregion

        #region FIELDS

        private readonly IGizmoClient _gizmoClient;
        private readonly NavigationService _navigationService;
        private readonly IOptionsMonitor<ClientNetworkOptions> _networkOptions;
        private readonly UserViewState _userViewState;
        private readonly ILogger<AvatarService> _logger;
        private readonly HttpClient _httpClient;

        private int _userId;

        #endregion

        #region PROPERTIES

        public string? Picture { get; private set; }

        public bool ShowNudge { get; private set; }

        /// <summary>
        /// Short human-readable reason the last operation failed, or null if
        /// it succeeded. Shown verbatim in ChangePictureDialog - "не
        /// получилось, попробуй позже" with no detail left the operator with
        /// nothing to go on when the proxy was unreachable.
        /// </summary>
        public string? LastError { get; private set; }

        #endregion

        #region EVENTS

        public event Action? Changed;

        /// <summary>
        /// Notifies subscribers that the avatar or nudge state changed.
        /// </summary>
        /// <remarks>
        /// Raised from HTTP continuations and login events, i.e. never on the UI thread, and
        /// straight into components. Subscribers are expected to marshal for themselves
        /// (DispatchStateHasChanged), but one throwing must not stop the others from being
        /// notified or bubble into the caller's event pipeline.
        /// </remarks>
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
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Avatar change subscriber failed.");
                }
            }
        }

        #endregion

        #region EVENT HANDLERS

        // Both handlers below are async void because that is the shape the client's events
        // require. That makes any exception escaping them fatal to the whole process - an
        // unhandled async void throw lands on the thread pool and trips the app domain handler
        // ("Client app domain unhandled exception. Client will exit."). An avatar is decoration;
        // it must never be able to end a customer's session, so both bodies are wrapped
        // wholesale.
        private async void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            try
            {
                if (e.State == LoginState.LoginCompleted)
                {
                    var profile = e.UserProfile;

                    if (profile is null)
                        return;

                    _userId = profile.Id;

                    // Guests have no persistent account/picture - nothing to fetch.
                    if (profile.IsGuest)
                        return;

                    await RefreshAsync(profile.Id, default);
                }
                else if (e.State == LoginState.LoggingOut)
                {
                    // Clear immediately - this is a shared kiosk PC. The next
                    // customer to log in on this station must never briefly
                    // see the previous customer's avatar or nudge banner while
                    // their own loads.
                    _userId = 0;
                    Picture = null;
                    ShowNudge = false;
                    RaiseChanged();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Avatar login state handling failed.");
            }
        }

        private async void OnUserViewStateChanged(object? sender, EventArgs e)
        {
            try
            {
                var id = _userViewState.Id;

                // Only act on a transition to a different real account - this
                // view state raises OnChange for every profile field edit too.
                if (id <= 0 || id == _userId || _userViewState.IsGuest)
                    return;

                _userId = id;

                await RefreshAsync(id, default);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Avatar user state handling failed.");
            }
        }

        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Resolves the avatar proxy's address: same machine as the Gizmo
        /// server, on <see cref="ProxyPort"/>.
        /// </summary>
        /// <remarks>
        /// This used to derive the host from NavigationService.GetBaseUri(),
        /// which is wrong on the desktop shell and was why nothing ever
        /// reached the proxy: inside a WPF BlazorWebView the Blazor app is
        /// served from the WebView2 virtual host, so BaseUri is
        /// "https://0.0.0.0/" - not the Gizmo server, not anything routable.
        /// The client's real server address is the one it already uses for
        /// every API call: ClientNetworkOptions.ServerUri (options.json ->
        /// "Network": { "ServerUri": ... }). BaseUri stays as the fallback
        /// purely for the Web host, where the shell IS served by the server
        /// and ServerUri is normally left empty.
        /// </remarks>
        private string? GetProxyBaseUrl()
        {
            var serverUri = _networkOptions.CurrentValue?.ServerUri;

            if (string.IsNullOrWhiteSpace(serverUri))
                serverUri = _navigationService.GetBaseUri();

            if (string.IsNullOrWhiteSpace(serverUri) || !Uri.TryCreate(serverUri, UriKind.Absolute, out var uri))
            {
                LastError = "Не настроен адрес сервера.";
                return null;
            }

            var host = uri.Host;

            // The WebView2 virtual host, if we did end up on the fallback.
            // Not routable as-is; on the desktop shell the only sensible
            // reading of it is "this machine".
            if (host is "0.0.0.0" or "[::]")
                host = "localhost";

            return $"http://{host}:{ProxyPort}";
        }

        /// <summary>
        /// Fetches the given user's avatar from the proxy and applies it to
        /// Picture. Any failure (proxy not running, no picture set, network
        /// error) quietly leaves Picture unset - a missing avatar is never
        /// a blocking error.
        /// </summary>
        public async Task RefreshAsync(int userId, CancellationToken cToken)
        {
            var baseUrl = GetProxyBaseUrl();
            if (baseUrl is null || userId <= 0)
                return;

            try
            {
                using var response = await _httpClient.GetAsync($"{baseUrl}/avatar/{userId}", cToken);

                if (!response.IsSuccessStatusCode)
                {
                    // 404 = no picture set for this user - not an error.
                    Picture = null;
                    RaiseChanged();

                    // Fire-and-forget: the nudge decision is a separate,
                    // slower-paced concern (its own delay before showing)
                    // and must never hold up the login flow this runs on.
                    _ = CheckNudgeAsync(userId);
                    return;
                }

                var bytes = await response.Content.ReadAsByteArrayAsync(cToken);
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";

                Picture = $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
                RaiseChanged();
            }
            catch (Exception ex)
            {
                // Avatar proxy not running / unreachable. Fail quiet and
                // keep the default icon - this must never block login.
                _logger.LogWarning(ex, "Avatar fetch failed for user {UserId} (proxy unreachable?).", userId);
            }
        }

        /// <summary>
        /// Asks the proxy's local index whether this login should nudge the
        /// user to add a photo (first login ever, then every few sessions -
        /// see AvatarIndex.should_show_nudge on the proxy side). Pure local
        /// lookup on the proxy, no extra call to the real Gizmo API.
        /// </summary>
        private async Task CheckNudgeAsync(int userId)
        {
            var baseUrl = GetProxyBaseUrl();
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

                ShowNudge = true;
                RaiseChanged();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Avatar nudge check failed for user {UserId} (proxy unreachable?).", userId);
            }
        }

        /// <summary>
        /// Hides the nudge banner. Called both when the user dismisses it
        /// directly and after it auto-hides itself on a timer.
        /// </summary>
        public void DismissNudge()
        {
            if (!ShowNudge)
                return;

            ShowNudge = false;
            RaiseChanged();
        }

        /// <summary>
        /// Uploads a new avatar for the currently logged-in user. The
        /// caller is expected to have already resized/compressed
        /// imageBytes client-side (see ChangePictureDialog) - this method
        /// does no processing of its own.
        /// </summary>
        public async Task<bool> UploadAsync(byte[] imageBytes, string contentType, CancellationToken cToken)
        {
            LastError = null;

            var baseUrl = GetProxyBaseUrl();
            if (baseUrl is null)
                return false;

            if (_userId <= 0)
            {
                LastError = "Не удалось определить аккаунт.";
                return false;
            }

            try
            {
                using var content = new ByteArrayContent(imageBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                using var response = await _httpClient.PostAsync($"{baseUrl}/avatar/{_userId}", content, cToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Avatar upload rejected by proxy: {Status}", response.StatusCode);
                    LastError = $"Сервер отклонил загрузку ({(int)response.StatusCode}).";
                    return false;
                }

                LastError = null;
                Picture = $"data:{contentType};base64,{Convert.ToBase64String(imageBytes)}";
                RaiseChanged();

                DismissNudge();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Avatar upload failed (proxy unreachable?).");
                LastError = $"Нет связи с {baseUrl} — служба аватарок не отвечает.";
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
