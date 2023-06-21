using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/notfound")]
    public partial class NotFoundPage : ComponentBase
    {
        [Inject]
        UserLoginStatusViewState UserLoginStatusViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        private void GoHome()
        {
            if (UserLoginStatusViewState.IsLoggedIn)
                NavigationService.NavigateTo(ClientRoutes.HomeRoute);
            else
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
        }
    }
}
