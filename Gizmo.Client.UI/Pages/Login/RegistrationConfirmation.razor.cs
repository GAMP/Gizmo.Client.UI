using Gizmo.Client.UI.View.Services;
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
        UserRegistrationConfirmationService UserRegistrationConfirmationService { get; set; }

        [Inject]
        UserRegistrationConfirmationMethodService UserRegistrationConfirmationMethodService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserRegistrationConfirmationMethodService.ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserRegistrationConfirmationMethodService.ViewState);
            base.Dispose();
        }
    }
}
