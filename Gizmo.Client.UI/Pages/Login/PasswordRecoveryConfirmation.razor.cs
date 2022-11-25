using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryConfirmationRoute)]
    public partial class PasswordRecoveryConfirmation : ComponentBase
    {
        [Inject]
        UserPasswordRecoveryConfirmationService UserPasswordRecoveryConfirmationService { get; set; }

        [Inject]
        UserLoginService UserLoginService { get; set; }

        [Inject]
        HostService HostService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }
    }
}
