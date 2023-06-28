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
        UserPasswordRecoveryViewService UserPasswordRecoveryService { get; set; }

        [Inject]
        UserPasswordRecoveryConfirmationViewService UserPasswordRecoveryConfirmationViewService { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        HostNumberViewService HostService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        UserPasswordRecoveryConfirmationViewState ViewState { get; set; }

        [Inject]
        UserPasswordRecoveryViewState UserPasswordRecoveryViewState { get; set; }

        [Inject]
        UserVerificationFallbackViewState UserVerificationFallbackViewState { get; set; }

        [Inject()]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState
        {
            get; init;
        }

        private string GetPlaceholder()
        {
            if (UserPasswordRecoveryViewState.CodeLength <= 3)
                return "123".Substring(0, UserPasswordRecoveryViewState.CodeLength);
            else if (UserPasswordRecoveryViewState.CodeLength == 4)
                return "12 34".Substring(0, UserPasswordRecoveryViewState.CodeLength + 1);
            else if (UserPasswordRecoveryViewState.CodeLength >= 5 && UserPasswordRecoveryViewState.CodeLength <= 6)
                return "123 456".Substring(0, UserPasswordRecoveryViewState.CodeLength + 1);
            else
                return "1234 5678".Substring(0, UserPasswordRecoveryViewState.CodeLength + 1);
        }

        public void OnCloseButtonClickHandler()
        {
            UserPasswordRecoveryConfirmationViewService.Reset();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserVerificationFallbackViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserVerificationFallbackViewState);
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
