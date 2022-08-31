using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenu : ComponentBase
    {
        public HeaderUserMenu()
        {
        }

        private bool _activeAppsIsOpen;
        private bool _notificationsIsOpen;

        [Inject]
        UserService UserService { get; set; }

        public void ToggleActiveApps()
        {
            _activeAppsIsOpen = !_activeAppsIsOpen;

            if (_activeAppsIsOpen)
            {
                _notificationsIsOpen = false;
            }
        }

        public void ToggleNotifications()
        {
            _notificationsIsOpen = !_notificationsIsOpen;

            if (_notificationsIsOpen)
            {
                _activeAppsIsOpen = false;
            }
        }
    }
}