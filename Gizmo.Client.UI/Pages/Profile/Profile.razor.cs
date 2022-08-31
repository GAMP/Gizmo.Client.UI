using Gizmo.Client.UI.View.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [DefaultRoute("/profile", DefaultRouteMatch = NavlinkMatch.Prefix)]
    [Route("/profile")]
    public partial class Profile : ComponentBase
    {
        [Inject]
        UserService UserService { get; set; }
    }
}
