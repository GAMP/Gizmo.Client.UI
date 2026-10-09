using Gizmo.Client.UI.Localization.Services;
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
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PasswordRecoverySetNewPasswordViewService PasswordRecoverySetNewPasswordViewService { get; set; }

        [Inject]
        PasswordRecoverySetNewPasswordViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; set; }

        private void OpenRegistration() => NavigationService.NavigateTo(ClientRoutes.RegistrationIndexRoute);

        public void OnCloseButtonClickHandler()
        {
            PasswordRecoverySetNewPasswordViewService.Reset();
        }

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
