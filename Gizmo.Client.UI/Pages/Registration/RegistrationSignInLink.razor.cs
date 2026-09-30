using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    public partial class RegistrationSignInLink : ComponentBase
    {
        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; } = builder => { };

        private void NavigateToLogin()
        {
            RegistrationSession.Clear();
            NavigationService.NavigateTo(ClientRoutes.LoginRoute);
        }
    }
}
