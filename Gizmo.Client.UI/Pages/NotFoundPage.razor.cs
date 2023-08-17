using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Pages
{
    [Route("/notfound")]
    public partial class NotFoundPage : ComponentBase
    {
        [Inject]
        UserLoginStatusViewState UserLoginStatusViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        IOptions<ClientHomeOptions> ClientHomeOptions { get; set; }

        private void GoHome()
        {
            if (UserLoginStatusViewState.IsLoggedIn)
            {
                if (!ClientHomeOptions.Value.Disabled)
                {
                    NavigationService.NavigateTo(ClientRoutes.HomeRoute);
                }
                else
                {
                    NavigationService.NavigateTo(ClientRoutes.ApplicationsRoute);
                }
            }
            else
            {
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
            }
        }
    }
}
