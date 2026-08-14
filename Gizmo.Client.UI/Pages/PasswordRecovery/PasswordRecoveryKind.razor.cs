using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryKindRoute)]
    public partial class PasswordRecoveryKind : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PasswordRecoveryKindViewService PasswordRecoveryKindViewService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }
    }
}
