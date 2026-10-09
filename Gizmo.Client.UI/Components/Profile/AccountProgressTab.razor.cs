using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class AccountProgressTab : ShellComponentBase
    {
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

        private Task LoadProgressAsync() => Task.WhenAll(ChallengesService.LoadAsync(), AchievementsService.LoadAsync());

        protected override void OnInitialized()
        {
            this.SubscribeChange(Ladder);
            this.SubscribeChange(Challenges);
            this.SubscribeChange(Achievements);

            if (Progress is null)
                DispatchWorkflow(LoadProgressAsync);
            else
                Progress.Ensure(LoadProgressAsync, PlayerProgress.Failed(Challenges, Achievements));

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
