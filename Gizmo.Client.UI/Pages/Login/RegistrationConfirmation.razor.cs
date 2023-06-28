using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.RegistrationConfirmationRoute)]
    public partial class RegistrationConfirmation : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationConfirmationViewService UserRegistrationConfirmationViewService { get; set; }

        [Inject]
        UserRegistrationConfirmationMethodViewState UserRegistrationConfirmationMethodViewState { get; set; }

        [Inject]
        UserRegistrationConfirmationViewState ViewState { get; set; }

        [Inject]
        UserVerificationFallbackViewState UserVerificationFallbackViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        private string GetPlaceholder()
        {
            if (UserRegistrationConfirmationMethodViewState.CodeLength <= 3)
                return "123".Substring(0, UserRegistrationConfirmationMethodViewState.CodeLength);
            else if (UserRegistrationConfirmationMethodViewState.CodeLength == 4)
                return "12 34".Substring(0, UserRegistrationConfirmationMethodViewState.CodeLength + 1);
            else if (UserRegistrationConfirmationMethodViewState.CodeLength >= 5 && UserRegistrationConfirmationMethodViewState.CodeLength <= 6)
                return "123 456".Substring(0, UserRegistrationConfirmationMethodViewState.CodeLength + 1);
            else
                return "1234 5678".Substring(0, UserRegistrationConfirmationMethodViewState.CodeLength + 1);
        }

        public void OnCloseButtonClickHandler()
        {
            UserRegistrationConfirmationViewService.Reset();
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
