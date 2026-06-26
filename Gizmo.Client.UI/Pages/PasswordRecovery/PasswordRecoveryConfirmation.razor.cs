using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryConfirmationRoute)]
    public partial class PasswordRecoveryConfirmation : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PasswordRecoveryConfirmationViewService PasswordRecoveryConfirmationViewService { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        PasswordRecoveryConfirmationViewState ViewState { get; set; }

        [Inject]
        IPasswordRecoverySessionService PasswordRecoverySession { get; set; }

        [Inject]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; init; }

        private async Task ResendCode()
        {
            await PasswordRecoveryConfirmationViewService.RestartTimerAsync();
        }

        public void OnCloseButtonClickHandler()
        {
            PasswordRecoveryConfirmationViewService.Reset();
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
