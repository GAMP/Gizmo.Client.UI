using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/registrationconfirmationmethod")]
    public partial class RegistrationConfirmationMethod : ComponentBase
    {
        [Inject]
        UserRegistrationConfirmationMethodService UserRegistrationConfirmationMethodService { get; set; }
    }
}
