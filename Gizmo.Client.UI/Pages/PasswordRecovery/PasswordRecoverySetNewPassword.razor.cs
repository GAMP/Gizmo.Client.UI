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
        PasswordRecoverySetNewPasswordViewService PasswordRecoverySetNewPasswordViewService { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        PasswordRecoverySetNewPasswordViewState ViewState { get; set; }

        [Inject]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; init; }

        public void OnCloseButtonClickHandler()
        {
            PasswordRecoverySetNewPasswordViewService.Reset();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserRegisterConfigurationViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(UserRegisterConfigurationViewState);

            base.Dispose();
        }
    }
}
