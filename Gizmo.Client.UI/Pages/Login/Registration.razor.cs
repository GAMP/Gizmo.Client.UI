using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/registration")]
    public partial class Registration : ComponentBase
    {
        [Inject]
        UserRegistrationService UserRegistrationService { get; set; }
    }
}
