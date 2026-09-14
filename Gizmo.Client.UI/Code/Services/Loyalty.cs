using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client.Options;
using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Messaging;
using Gizmo.Web.Api.Models;

//The user surface of the API: same class names as the operator surface, other routes.
using UserAchievementsClient = Gizmo.Web.Api.User.Clients.AchievementsWebApiClient;
using UserLadderClient = Gizmo.Web.Api.User.Clients.AchievementLadderWebApiClient;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// The signed-in customer's ladder standing, achievements and challenges, kept for the
    /// whole shell.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The server (3.0.95+) serves all of it through the user surface of the Web API
    /// (<c>api/user/v3/...</c>); the client's own protocol carries none of it. The REST
    /// clients are the host's, already registered with the server's address and the
    /// customer's bearer token, so this is the only place in the shell that talks to
    /// them. Everything on screen reads <see cref="State"/>, a snapshot replaced whole
    /// on every change, and re-renders on <see cref="Changed"/>.
    /// </para>
    /// <para>
    /// Static, like <see cref="ShellActivity"/>: the host registers nothing from the skin
    /// assembly except the dialog and notification services, and the state has to
    /// outlive any page. <see cref="Attach"/> is called once from <c>App</c>.
    /// </para>
    /// <para>
    /// Refreshed on sign-in and whenever the server pushes an achievement event to this
    /// client (<c>IGizmoClient.OnAPIEventMessage</c>); never polled. Guests get no token
    /// from the host, so for them - and for a club that has configured nothing - the
    /// snapshot simply says the feature is not there.
    /// </para>
    /// </remarks>
    public static class Loyalty
    {
        /// <summary>
        /// How long after an event the reload waits, so a burst (an achievement and the
        /// challenge it completes) costs one round trip.
        /// </summary>
        private static readonly TimeSpan EventSettle = TimeSpan.FromMilliseconds(1200);

        /// <summary>
        /// How long a first 401 after sign-in is given before it counts: the host fetches
        /// the customer's bearer token on its own after raising the login event.
        /// </summary>
        private static readonly TimeSpan TokenSettle = TimeSpan.FromSeconds(3);

        private static readonly object _gate = new();

        private static IServiceProvider _services;
        private static IGizmoClient _client;
        private static ILogger _logger;
        private static bool _attached;

        private static CancellationTokenSource _session;
        private static Timer _eventTimer;
        private static readonly List<IAPIEventMessage> _pendingEvents = new();
        private static bool _hintShown;

        /// <summary>The current snapshot. Never null.</summary>
        public static LoyaltySnapshot State { get; private set; } = LoyaltySnapshot.None;

        /// <summary>Raised on the client's threads after <see cref="State"/> is replaced.</summary>
        public static event Action Changed;

        /// <summary>
        /// Raised for a piece of news worth a word on screen: an achievement earned, a
        /// challenge completed, a level change, a reward's status. After the snapshot
        /// behind it has been refreshed, so names resolve.
        /// </summary>
        public static event Action<LoyaltyNews> News;

        /// <summary>
        /// Hooks the client's events. Idempotent; the first call wins.
        /// </summary>
        public static void Attach(IServiceProvider services)
        {
            lock (_gate)
            {
                if (_attached || services is null)
                    return;

                _services = services;
                _logger = services.GetService<ILoggerFactory>()?.CreateLogger("Grafit.Loyalty");
                _client = services.GetService<IGizmoClient>();

                if (_client is null)
                    return;

                _client.LoginStateChange += OnLoginStateChange;
                _client.OnAPIEventMessage += OnApiEventMessage;
                _client.ConnectionStateChange += OnConnectionStateChange;
                _attached = true;
            }

            //Already signed in when the shell starts (a WebView recreated mid-session).
            if (_client.IsUserLoggedIn)
                StartSession(services.GetService<UserViewState>()?.IsGuest ?? false);
        }

        /// <summary>
        /// Absolute address of a server file by its guid - achievement pictures, level
        /// emblems - or null when there is none.
        /// </summary>
        public static string ImageUrl(Guid? guid)
        {
            if (!guid.HasValue || guid.Value == Guid.Empty || _services is null)
                return null;

            var server = _services.GetService<IOptionsMonitor<ClientNetworkOptions>>()?.CurrentValue?.ServerUri;
            if (string.IsNullOrWhiteSpace(server))
                return null;

            return server.TrimEnd('/') + "/files/" + guid.Value.ToString("D");
        }

        /// <summary>
        /// The sign-in hint is offered once per session; the pill that shows it calls this
        /// when it has been on screen, so a page change does not bring it back.
        /// </summary>
        public static bool TakeHint()
        {
            lock (_gate)
            {
                if (_hintShown || !State.IsAvailable)
                    return false;

                _hintShown = true;
                return true;
            }
        }

        /// <summary>
        /// Reloads everything from the server for the current session.
        /// </summary>
        public static Task RefreshAsync() => LoadAsync(_session?.Token ?? CancellationToken.None, Array.Empty<IAPIEventMessage>());

        #region EVENTS

        //All three arrive on the client's threads and must not throw: an unobserved
        //exception here takes the whole client down.
        private static void OnLoginStateChange(object sender, UserLoginStateChangeEventArgs e)
        {
            try
            {
                if (e.State == LoginState.LoggedIn)
                    StartSession(e.UserProfile?.IsGuest ?? false);
                else if (e.State == LoginState.LoggedOut || e.State == LoginState.LoggingOut)
                    EndSession();
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Loyalty: login state change failed.");
            }
        }

        private static void OnApiEventMessage(object sender, IAPIEventMessage e)
        {
            if (e is not UserAchievementEventMessageBase)
                return;

            try
            {
                lock (_gate)
                {
                    if (_session is null)
                        return;

                    _pendingEvents.Add(e);

                    //One reload per burst: the timer is pushed back by each event.
                    _eventTimer ??= new Timer(OnEventTimer, null, Timeout.Infinite, Timeout.Infinite);
                    _eventTimer.Change(EventSettle, Timeout.InfiniteTimeSpan);
                }
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Loyalty: could not schedule the reload for an event.");
            }
        }

        private static void OnConnectionStateChange(object sender, ConnectionStateEventArgs e)
        {
            try
            {
                //A reload that failed while the link was down is retried when it is back.
                if (e.IsConnected && _session is not null && State.LastError is not null)
                    _ = LoadAsync(_session.Token, Array.Empty<IAPIEventMessage>());
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Loyalty: reconnect handling failed.");
            }
        }

        private static void OnEventTimer(object _)
        {
            IAPIEventMessage[] events;
            CancellationToken token;

            lock (_gate)
            {
                events = _pendingEvents.ToArray();
                _pendingEvents.Clear();
                token = _session?.Token ?? CancellationToken.None;
            }

            if (events.Length > 0 && !token.IsCancellationRequested)
                _ = LoadAsync(token, events);
        }

        #endregion

        #region SESSION

        private static void StartSession(bool isGuest)
        {
            CancellationToken token;

            lock (_gate)
            {
                _session?.Cancel();
                _session = new CancellationTokenSource();
                _hintShown = false;
                _pendingEvents.Clear();
                token = _session.Token;

                if (isGuest)
                {
                    //The host gives a guest no bearer token; every call would be a 401.
                    _session = null;
                    Publish(LoyaltySnapshot.None);
                    return;
                }

                Publish(LoyaltySnapshot.None with { IsLoading = true });
            }

            _ = LoadAsync(token, Array.Empty<IAPIEventMessage>());
        }

        private static void EndSession()
        {
            lock (_gate)
            {
                _session?.Cancel();
                _session = null;
                _pendingEvents.Clear();
                _hintShown = false;
            }

            Publish(LoyaltySnapshot.None);
        }

        #endregion

        #region LOADING

        private static async Task LoadAsync(CancellationToken token, IAPIEventMessage[] events)
        {
            if (_services is null || token.IsCancellationRequested)
                return;

            LoyaltySnapshot next;

            try
            {
                next = await FetchAsync(token);

                if (next is null)
                {
                    //Refused once: the token may simply not be there yet. Once more, later.
                    await Task.Delay(TokenSettle, token);
                    next = await FetchAsync(token) ?? LoyaltySnapshot.None;
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Loyalty: reload failed.");
                next = State with { IsLoading = false, LastError = exception };
            }

            if (token.IsCancellationRequested)
                return;

            var previous = State;
            Publish(next);

            if (events.Length > 0)
                Announce(previous, next, events);
        }

        /// <summary>
        /// One round of the three reads; null when the server refused them all for want
        /// of a user token (no user behind this session, or not yet).
        /// </summary>
        private static async Task<LoyaltySnapshot> FetchAsync(CancellationToken token)
        {
            using var scope = _services.CreateScope();
            var achievementsClient = scope.ServiceProvider.GetRequiredService<UserAchievementsClient>();
            var ladderClient = scope.ServiceProvider.GetRequiredService<UserLadderClient>();

            //Three independent reads; a failure of one is not a failure of the rest, and
            //the parts the server does not have (no ladder configured) come back empty
            //rather than as errors.
            var standingTask = Guard(ladderClient.GetStandingAsync(new LadderStandingFilter { Progress = true }, token), "standing");
            var achievementsTask = Guard(achievementsClient.GetAchievementsAsync(new UserAchievementsFilter { Progress = true, IncludeUnavailable = false }, token), "achievements");
            var challengesTask = Guard(achievementsClient.GetChallengesAsync(new UserAchievementChallengesFilter { Progress = true, IncludeUnavailable = false }, token), "challenges");

            await Task.WhenAll(standingTask, achievementsTask, challengesTask);

            var (standing, standingError) = standingTask.Result;
            var (achievements, achievementsError) = achievementsTask.Result;
            var (challenges, challengesError) = challengesTask.Result;

            var unauthorized = new[] { standingError, achievementsError, challengesError }
                .OfType<WebApiClientException>()
                .Any(a => a.HttpStatusCode == HttpStatusCode.Unauthorized || a.HttpStatusCode == HttpStatusCode.Forbidden);

            if (unauthorized)
                return null;

            return LoyaltySnapshot.From(standing, achievements, challenges,
                standingError ?? achievementsError ?? challengesError);
        }

        private static async Task<(T Result, Exception Error)> Guard<T>(Task<T> call, string what) where T : class
        {
            try
            {
                return (await call, null);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (WebApiClientException exception) when (exception.HttpStatusCode == HttpStatusCode.NotFound)
            {
                //A surface the server does not have (older release, or nothing configured).
                return (null, null);
            }
            catch (Exception exception)
            {
                _logger?.LogWarning(exception, "Loyalty: could not load {what}.", what);
                return (null, exception);
            }
        }

        private static void Publish(LoyaltySnapshot snapshot)
        {
            State = snapshot;

            try
            {
                Changed?.Invoke();
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "Loyalty: a Changed handler threw.");
            }
        }

        #endregion

        #region NEWS

        private static void Announce(LoyaltySnapshot before, LoyaltySnapshot after, IAPIEventMessage[] events)
        {
            foreach (var e in events)
            {
                LoyaltyNews news = e switch
                {
                    UserAchievementLevelChangedEventMessage level => LevelNews(after, level),
                    UserAchievementCompletedEventMessage earned => AchievementNews(after, earned),
                    UserAchievementChallengeCompletedEventMessage done => ChallengeNews(after, done),
                    UserAchievementRewardStatusChangedEventMessage reward => RewardNews(after, reward),
                    _ => null,
                };

                if (news is null)
                    continue;

                try
                {
                    News?.Invoke(news);
                }
                catch (Exception exception)
                {
                    _logger?.LogError(exception, "Loyalty: a News handler threw.");
                }
            }
        }

        private static LoyaltyNews LevelNews(LoyaltySnapshot s, UserAchievementLevelChangedEventMessage e)
        {
            var name = s.Standing?.Levels.FirstOrDefault(l => l.Rank == e.ToRank)?.Name ?? string.Empty;
            var up = e.ToRank > e.FromRank;
            return new LoyaltyNews(
                up ? LoyaltyNewsKind.LevelUp : LoyaltyNewsKind.LevelDown,
                ShellStringOverrides.Get(up ? ShellStringOverrides.LOYALTY_NEWS_LEVEL_UP : ShellStringOverrides.LOYALTY_NEWS_LEVEL_DOWN),
                name);
        }

        private static LoyaltyNews AchievementNews(LoyaltySnapshot s, UserAchievementCompletedEventMessage e)
        {
            var name = s.Achievements.FirstOrDefault(a => a.AchievementId == e.AchievementId)?.Name ?? string.Empty;
            return new LoyaltyNews(LoyaltyNewsKind.Achievement,
                ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_NEWS_ACHIEVEMENT), name);
        }

        private static LoyaltyNews ChallengeNews(LoyaltySnapshot s, UserAchievementChallengeCompletedEventMessage e)
        {
            var name = s.Challenges.FirstOrDefault(c => c.ChallengeId == e.ChallengeId)?.Name ?? string.Empty;
            return new LoyaltyNews(LoyaltyNewsKind.Challenge,
                ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_NEWS_CHALLENGE), name);
        }

        private static LoyaltyNews RewardNews(LoyaltySnapshot s, UserAchievementRewardStatusChangedEventMessage e)
        {
            var name = s.Challenges.FirstOrDefault(c => c.ChallengeId == e.ChallengeId)?.Name ?? string.Empty;
            return e.Status switch
            {
                AchievementChallengeRewardStatus.AwaitingClaim => new LoyaltyNews(LoyaltyNewsKind.Reward,
                    ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_NEWS_REWARD_WAITING), name),
                AchievementChallengeRewardStatus.Delivered or AchievementChallengeRewardStatus.Claimed => new LoyaltyNews(LoyaltyNewsKind.Reward,
                    ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_NEWS_REWARD_GIVEN), name),
                _ => null,
            };
        }

        #endregion
    }

    public enum LoyaltyNewsKind
    {
        Achievement,
        Challenge,
        LevelUp,
        LevelDown,
        Reward,
    }

    /// <summary>One thing worth telling the customer: a title and the name it is about.</summary>
    public sealed record LoyaltyNews(LoyaltyNewsKind Kind, string Title, string Name);
}
