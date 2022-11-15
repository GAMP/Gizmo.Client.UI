using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/registrationconfirmation")]
    public partial class RegistrationConfirmation : ComponentBase
    {
        [Inject]
        UserRegistrationConfirmationService UserRegistrationConfirmationService { get; set; }

        [Inject]
        UserRegistrationConfirmationMethodService UserRegistrationConfirmationMethodService { get; set; }
    }
}
