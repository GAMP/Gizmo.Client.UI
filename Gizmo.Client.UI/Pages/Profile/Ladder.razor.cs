using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserLadderRoute)]
    public partial class Ladder : ShellComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserLadderViewState ViewState { get; set; }

        [Inject]
        UserLadderViewService Service { get; set; }

        [Inject]
        UserChallengesViewState Challenges { get; set; }

        [Inject]
        UserChallengesViewService ChallengesService { get; set; }

        [Inject]
        UserAchievementsViewState Achievements { get; set; }

        [Inject]
        UserAchievementsViewService AchievementsService { get; set; }

        [CascadingParameter]
        ProgressLoading Progress { get; set; }

        private string ChallengesCountText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PROGRESS_OF,
            Challenges.Challenges.Count(a => a.IsDone), Challenges.Challenges.Count());

        private string AchievementsCountText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PROGRESS_OF,
            Achievements.Achievements.Count(a => a.IsEarned), Achievements.Achievements.Count());

        private bool IsEmpty =>
            !ViewState.IsLoading && !ViewState.HasError && !ViewState.HasStanding &&
            !Challenges.IsLoading && !Challenges.Challenges.Any() &&
            !Achievements.IsLoading && !Achievements.Achievements.Any();

        private Task LoadProgressAsync() => Task.WhenAll(ChallengesService.LoadAsync(), AchievementsService.LoadAsync());

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(Challenges);
            this.SubscribeChange(Achievements);

            if (Progress is null)
                DispatchWorkflow(LoadProgressAsync);
            else
                Progress.Refresh(LoadProgressAsync);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Achievements);
            this.UnsubscribeChange(Challenges);
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        private void OnHistoryOpenChanged(bool isOpen)
        {
            if (!isOpen)
                Service.ClearSelection();
        }
    }
}
