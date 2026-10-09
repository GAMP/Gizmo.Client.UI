using Gizmo.UI;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [DefaultRoute(ClientRoutes.UserProfileRoute, DefaultRouteMatch = NavlinkMatch.Prefix)]
    [Route(ClientRoutes.UserProfileRoute)]
    public partial class Profile : CustomDOMComponentBase
    {
    }
}
