using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoverySetNewPasswordRoute)]
    public partial class PasswordRecoverySetNewPassword : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserPasswordRecoverySetNewPasswordViewStateService UserPasswordRecoverySetNewPasswordService { get; set; }

        [Inject]
        UserPasswordRecoverySetNewPasswordViewState ViewState { get; set; }

        [Inject]
        UserLoginViewStateService UserLoginService { get; set; }

        [Inject]
        HostNumberViewStateService HostService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
