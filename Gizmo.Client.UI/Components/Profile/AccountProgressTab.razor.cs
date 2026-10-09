using System.Threading;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class AccountProgressTab : ShellComponentBase
    {
        private readonly CancellationTokenSource _lifetime = new();

        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject] UserLadderSummaryViewState Ladder { get; set; }
        [Inject] UserChallengesViewState Challenges { get; set; }
        [Inject] UserChallengesViewService ChallengesService { get; set; }
        [Inject] UserAchievementsViewState Achievements { get; set; }
        [Inject] UserAchievementsViewService AchievementsService { get; set; }

        [CascadingParameter] ProgressLoading Progress { get; set; }

        [Parameter]
        public string TabClass { get; set; }

        private bool ProgressOn => PlayerProgress.Any(Ladder, Challenges, Achievements);

        protected override void OnInitialized()
        {
            this.SubscribeChange(Ladder);
            this.SubscribeChange(Challenges);
            this.SubscribeChange(Achievements);

            if (Progress?.TryStart() != false)
            {
                DispatchWorkflow(() => ChallengesService.LoadAsync(_lifetime.Token));
                DispatchWorkflow(() => AchievementsService.LoadAsync(_lifetime.Token));
            }

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
