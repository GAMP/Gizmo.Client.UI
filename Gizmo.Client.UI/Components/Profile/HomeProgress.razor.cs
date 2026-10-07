using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Linq;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
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
        [Inject] UserChallengesViewService ChallengesService { get; set; }
        [Inject] UserAchievementsViewState Achievements { get; set; }
        [Inject] UserAchievementsViewService AchievementsService { get; set; }
        [Inject] NavigationService NavigationService { get; set; }

        private readonly CancellationTokenSource _lifetime = new();

        private bool HasLadder => Ladder.HasLevel;

        private bool ProgressOn => HasLadder || TotalChallenges > 0 || TotalAchievements > 0;

        private int EarnedAchievements => Achievements.Achievements.Count(a => a.IsEarned);

        private int TotalAchievements => Achievements.Achievements.Count();

        private int DoneChallenges => Challenges.Challenges.Count(a => a.IsDone);

        private int TotalChallenges => Challenges.Challenges.Count();

        private const int STEPS = 3;

        private bool HeadlineIsAchievements => !HasLadder && TotalAchievements > 0;

        private bool HeadlineIsChallenges => !HasLadder && TotalAchievements == 0 && TotalChallenges > 0;

        private decimal LinePercent
        {
            get
            {
                if (HasLadder)
                    return Ladder.TopBarProgressPercent;

                if (HeadlineIsAchievements)
                    return 100m * EarnedAchievements / TotalAchievements;

                return HeadlineIsChallenges ? 100m * DoneChallenges / TotalChallenges : 0m;
            }
        }

        private string LineValue => LinePercent.ToString("0.##", CultureInfo.InvariantCulture);

        private bool ShowLine => HasLadder ? Ladder.ShowTopBarProgress : HeadlineIsAchievements || HeadlineIsChallenges;

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

        private sealed record Step(string Title, string Note, string Reward, string RewardIcon);

        private IReadOnlyList<Step> Steps
        {
            get
            {
                var steps = new List<Step>();

                foreach (var challenge in Challenges.Challenges.Where(a => !a.IsDone && !a.IsEnded))
                {
                    var next = challenge.Requirements.FirstOrDefault(a => !a.IsMet);

                    if (next is null)
                        continue;

                    var reward = challenge.Rewards.FirstOrDefault();
                    var note = string.IsNullOrEmpty(challenge.WindowText)
                        ? challenge.Name
                        : $"{challenge.Name} · {challenge.WindowText}";

                    steps.Add(new Step(next.Name, note, reward?.Text, RewardIcon(reward)));
                }

                foreach (var achievement in Achievements.Achievements
                    .Where(a => !a.IsEarned && a.ShowProgressBar)
                    .OrderByDescending(a => a.ProgressPercent))
                {
                    var note = string.IsNullOrEmpty(achievement.CountText)
                        ? null
                        : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PROGRESS_ACHIEVEMENT, achievement.CountText);

                    steps.Add(new Step(achievement.Name, note, null, null));
                }

                return steps.Take(STEPS).ToList();
            }
        }

        private static string RewardIcon(UserChallengeRewardViewState reward) => reward?.Kind switch
        {
            ChallengeRewardKind.Points => "ph-fill ph-coins giz-home-progress__coin",
            ChallengeRewardKind.Time => "ph-fill ph-clock",
            _ => "ph-fill ph-gift",
        };

        private string Vendor(string key) => GrafitLocalization.GetString(key);

        private string Of(int count, int total) => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PROGRESS_OF, count, total);

        private void OpenProgress() => NavigationService.NavigateTo(ClientRoutes.UserLadderRoute);

        protected override void OnInitialized()
        {
            this.SubscribeChange(Ladder);
            this.SubscribeChange(Challenges);
            this.SubscribeChange(Achievements);

            if (!Challenges.IsLoading)
                DispatchWorkflow(() => ChallengesService.LoadAsync(_lifetime.Token));

            if (!Achievements.IsLoading)
                DispatchWorkflow(() => AchievementsService.LoadAsync(_lifetime.Token));

            base.OnInitialized();
        }

        public override void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();

            this.UnsubscribeChange(Achievements);
            this.UnsubscribeChange(Challenges);
            this.UnsubscribeChange(Ladder);

            base.Dispose();
        }
    }
}
