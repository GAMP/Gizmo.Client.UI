using Gizmo.Client;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserChallengesRoute)]
    public partial class Challenges : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }
    }
}
