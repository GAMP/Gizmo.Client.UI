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
        UserRegistrationConfirmationViewService UserRegistrationConfirmationService { get; set; }

        [Inject]
        UserRegistrationConfirmationViewState ViewState { get; set; }

        [Inject]
        UserVerificationFallbackViewState UserVerificationFallbackViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

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
