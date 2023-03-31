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
        UserLoginViewStateService UserLoginService { get; set; }

        [Inject]
        UserPasswordRecoverySetNewPasswordViewState ViewState { get; set; }

        [Inject()]
        HostConfigurationViewState HostConfigurationViewState
        {
            get; init;
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(HostConfigurationViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(HostConfigurationViewState);

            base.Dispose();
        }
    }
}
