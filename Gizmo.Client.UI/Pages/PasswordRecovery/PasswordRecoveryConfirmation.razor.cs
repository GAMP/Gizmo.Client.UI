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

        private string GetPlaceholder()
        {
            if (PasswordRecoverySession.CodeLength <= 0)
                return string.Empty;

            if (PasswordRecoverySession.CodeLength <= 3)
                return "123".Substring(0, PasswordRecoverySession.CodeLength);
            else if (PasswordRecoverySession.CodeLength == 4)
                return "12 34".Substring(0, PasswordRecoverySession.CodeLength + 1);
            else if (PasswordRecoverySession.CodeLength >= 5 && PasswordRecoverySession.CodeLength <= 6)
                return "123 456".Substring(0, PasswordRecoverySession.CodeLength + 1);
            else
                return "1234 5678".Substring(0, PasswordRecoverySession.CodeLength + 1);
        }

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
