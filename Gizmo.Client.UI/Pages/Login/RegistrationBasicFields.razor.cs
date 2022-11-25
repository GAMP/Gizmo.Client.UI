using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.RegistrationBasicFieldsRoute)]
    public partial class RegistrationBasicFields : ComponentBase
    {
        [Inject]
        UserRegistrationBasicFieldsService UserRegistrationBasicFieldsService { get; set; }
    }
}
