using System;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    /// <summary>
    /// The sign-in hint about the level ring. Offered once per session, for ten seconds,
    /// as soon as the loyalty data has arrived.
    /// </summary>
    public partial class LoyaltyHint : CustomDOMComponentBase
    {
        private static readonly TimeSpan ShowFor = TimeSpan.FromSeconds(10);

        [Inject] NavigationService NavigationService { get; set; }

        private Timer _timer;
        private bool _open;

        protected bool IsOpen => _open;

        //The level when there is a ladder; the count of achievements (or of challenges,
        //when the club runs only those) otherwise.
        protected string Title
        {
            get
            {
                var s = Loyalty.State;

                if (s.HasLadder && s.CurrentLevel is not null)
                    return ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_HINT_TITLE, s.CurrentLevel.Name);

                if (s.HasAchievements)
                    return ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_ACHIEVEMENTS) + " " +
                           ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_OF, s.EarnedAchievements, s.Achievements.Count);

                if (s.HasChallenges)
                    return ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_CHALLENGES) + " " +
                           ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_OF, s.DoneChallenges, s.Challenges.Count);

                return ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_HOME_CAP);
            }
        }

        //One invitation, nothing else: the standing is on the home tile and the tab.
        protected string Subtitle => ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_HINT_MORE);

        protected override void OnInitialized()
        {
            Loyalty.Changed += OnLoyaltyChanged;

            TryShow();

            base.OnInitialized();
        }

        public override void Dispose()
        {
            Loyalty.Changed -= OnLoyaltyChanged;

            _timer?.Dispose();
            _timer = null;

            base.Dispose();
        }

        private void OnLoyaltyChanged() => DispatchWorkflow(() =>
        {
            TryShow();
            return Task.CompletedTask;
        });

        private void TryShow()
        {
            if (_open || !Loyalty.TakeHint())
                return;

            _open = true;
            _timer?.Dispose();
            _timer = new Timer(_ => DispatchWorkflow(() =>
            {
                _open = false;
                StateHasChanged();
                return Task.CompletedTask;
            }), null, ShowFor, Timeout.InfiniteTimeSpan);

            DispatchStateHasChanged();
        }

        private void Open()
        {
            _open = false;
            _timer?.Dispose();
            _timer = null;

            NavigationService.NavigateTo(ClientRoutes.UserProfileRoute + "/progress");
        }
    }
}
