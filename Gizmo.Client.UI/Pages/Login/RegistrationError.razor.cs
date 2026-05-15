using Gizmo.Client;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.RegistrationErrorRoute)]
    public partial class RegistrationError : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public void GoBack() => NavigationManager.NavigateTo(ClientRoutes.RegistrationProvidersRoute);

        public void TryAgain() => NavigationManager.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
    }
}
