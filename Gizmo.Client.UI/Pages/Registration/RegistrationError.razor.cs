using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationErrorRoute)]
    public partial class RegistrationError : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        public void GoBack()
        {
            RegistrationSession.SetShowAllProviders(true);
            NavigationManager.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
        }

        public void TryAgain() => NavigationManager.NavigateTo(ClientRoutes.RegistrationRedirectRoute);
    }
}
