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

        [Inject]
        UserPasswordRecoveryMethodService UserPasswordRecoveryMethodService { get; set; }

        public bool ChangeEmailIsOpen { get; set; }

        public bool ChangeMobileIsOpen { get; set; }

        public bool ChangePasswordIsOpen { get; set; }

        private void OnClickUpdateEmailButtonHandler()
        {
            ChangePasswordIsOpen = true;
        }

        private void OnClickUpdateMobileButtonHandler()
        {
            ChangeEmailIsOpen = true;
        }

        private void OnClickChangePasswordButtonHandler()
        {
            ChangePasswordIsOpen = true;
        }
    }
}
