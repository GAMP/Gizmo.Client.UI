using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/passwordrecovery")]
    public partial class PasswordRecovery : ComponentBase
    {
        [Inject]
        UserPasswordRecoveryService UserPasswordRecoveryService { get; set; }
    }
}
