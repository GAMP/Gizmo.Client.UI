using Gizmo.Client;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserLadderRoute)]
    public partial class Ladder : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }
    }
}
