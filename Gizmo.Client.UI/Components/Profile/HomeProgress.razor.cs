using System.Globalization;
using System.Linq;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class HomeProgress : ShellComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject] UserLadderSummaryViewState Ladder { get; set; }
        [Inject] UserChallengesViewState Challenges { get; set; }
        [Inject] UserAchievementsViewState Achievements { get; set; }
        [Inject] NavigationService NavigationService { get; set; }

        private bool HasLadder => Ladder.HasLevel;

        private int EarnedAchievements => Achievements.Achievements.Count(a => a.IsEarned);

        private int TotalAchievements => Achievements.Achievements.Count();

        private int DoneChallenges => Challenges.Challenges.Count(a => a.IsDone);

        private int TotalChallenges => Challenges.Challenges.Count();

        private bool HeadlineIsAchievements => !HasLadder && TotalAchievements > 0;

        private bool HeadlineIsChallenges => !HasLadder && TotalAchievements == 0 && TotalChallenges > 0;

        private bool LinkIsAchievements => HasLadder && TotalAchievements > 0;

        private bool LinkIsChallenges => TotalChallenges > 0 && (HasLadder ? TotalAchievements == 0 : HeadlineIsAchievements);

        private string Caption => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PROGRESS_HOME_CAP);

        private string LinkText
        {
            get
            {
                if (LinkIsAchievements)
                    return $"{Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS))} {Of(EarnedAchievements, TotalAchievements)}";

                if (LinkIsChallenges)
                    return $"{Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES))} {Of(DoneChallenges, TotalChallenges)}";

                return null;
            }
        }

        private string LinkRoute => LinkIsAchievements ? ClientRoutes.UserAchievementsRoute : ClientRoutes.UserChallengesRoute;

        private decimal RingPercent
        {
            get
            {
                if (HasLadder)
                    return Ladder.ShowTopBarProgress ? Ladder.TopBarProgressPercent : 0m;

                if (HeadlineIsAchievements)
                    return 100m * EarnedAchievements / TotalAchievements;

                if (HeadlineIsChallenges)
                    return 100m * DoneChallenges / TotalChallenges;

                return 0m;
            }
        }

        private string RingValue => RingPercent.ToString("0.##", CultureInfo.InvariantCulture);

        private bool ShowLine => HasLadder && Ladder.ShowTopBarProgress;

        private string PlainIcon => HeadlineIsChallenges ? "ph-flag-checkered" : "ph-trophy";

        private string Title
        {
            get
            {
                if (HasLadder)
                    return Ladder.LevelName;

                return Vendor(HeadlineIsChallenges
                    ? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES)
                    : nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS));
            }
        }

        private string Subtitle
        {
            get
            {
                if (HasLadder)
                    return Ladder.HeaderStatusText;

                return HeadlineIsChallenges
                    ? Of(DoneChallenges, TotalChallenges)
                    : Of(EarnedAchievements, TotalAchievements);
            }
        }

        private UserChallengeViewState NextChallenge => Challenges.Challenges
            .FirstOrDefault(a => !a.IsDone && !a.IsEnded && a.Requirements.Any(r => !r.IsMet));

        private string NextStepName => NextChallenge?.Requirements.FirstOrDefault(r => !r.IsMet)?.Name;

        private string NextReward => NextChallenge?.Rewards.FirstOrDefault()?.Text;

        private string Vendor(string key) => GrafitLocalization.GetString(key);

        private string Of(int count, int total) => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PROGRESS_OF, count, total);

        private void OpenProgress() => NavigationService.NavigateTo(ClientRoutes.UserLadderRoute);

        protected override void OnInitialized()
        {
            this.SubscribeChange(Ladder);
            this.SubscribeChange(Challenges);
            this.SubscribeChange(Achievements);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Achievements);
            this.UnsubscribeChange(Challenges);
            this.UnsubscribeChange(Ladder);

            base.Dispose();
        }
    }
}
