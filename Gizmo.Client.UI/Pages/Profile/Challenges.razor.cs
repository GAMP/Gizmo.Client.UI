using Gizmo.Client;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserChallengesRoute)]
    public partial class Challenges : ShellComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserChallengesViewState ViewState { get; set; }

        [Inject]
        UserAchievementsViewService AchievementsService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            DispatchWorkflow(() => AchievementsService.LoadAsync());

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
