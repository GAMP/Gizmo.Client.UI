using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/passwordrecoveryconfirmation")]
    public partial class PasswordRecoveryConfirmation : ComponentBase
    {
        [Inject]
        UserPasswordRecoveryConfirmationService UserPasswordRecoveryConfirmationService { get; set; }
    }
}
