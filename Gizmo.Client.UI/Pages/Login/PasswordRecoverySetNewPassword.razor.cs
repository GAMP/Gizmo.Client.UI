using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/passwordrecoverysetnewpassword")]
    public partial class PasswordRecoverySetNewPassword : ComponentBase
    {
        [Inject]
        UserPasswordRecoverySetNewPasswordService UserPasswordRecoverySetNewPasswordService { get; set; }

        [Inject]
        UserLoginService UserLoginService { get; set; }
    }
}
