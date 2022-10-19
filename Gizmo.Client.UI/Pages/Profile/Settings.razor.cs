using Gizmo.Client.UI.View.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [DefaultRoute("/settings", DefaultRouteMatch = NavlinkMatch.Prefix)]
    [Route("/settings")]
    public partial class Settings : ComponentBase
    {
        [Inject]
        UserSettingsService UserSettingsService { get; set; }

        public bool ChangePasswordIsOpen { get; set; }

        private void OnClickChangePasswordButtonHandler()
        {
            ChangePasswordIsOpen = true;
        }
    }
}
