using System.Linq;
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

        [Parameter]
        public string TabClass { get; set; }

        private bool ProgressOn => Ladder.HasLevel || Challenges.Challenges.Any() || Achievements.Achievements.Any();

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
