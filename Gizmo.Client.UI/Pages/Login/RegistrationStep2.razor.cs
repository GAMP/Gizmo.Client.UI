using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/registrationstep2")]
    public partial class RegistrationStep2 : ComponentBase
    {
        [Inject]
        UserRegistrationStep2Service UserRegistrationStep2Service { get; set; }
    }
}
