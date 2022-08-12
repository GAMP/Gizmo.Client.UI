using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/registrationadditionalfields")]
    public partial class RegistrationAdditionalFields : ComponentBase
    {
        [Inject]
        UserRegistrationAdditionalFieldsService UserRegistrationAdditionalFieldsService { get; set; }
    }
}
