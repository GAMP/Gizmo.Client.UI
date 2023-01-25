using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.RegistrationBasicFieldsRoute)]
    public partial class RegistrationBasicFields : ComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationConfirmationMethodService UserRegistrationConfirmationMethodService { get; set; }

        [Inject]
        UserRegistrationBasicFieldsService UserRegistrationBasicFieldsService { get; set; }
    }
}
