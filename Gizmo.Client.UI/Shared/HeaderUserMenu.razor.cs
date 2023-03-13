using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenu : CustomDOMComponentBase
    {
        private bool _activeAppsIsOpen;
        private bool _notificationsIsOpen;
        private bool _userLinksIsOpen;

        public void ToggleActiveApps()
        {
            _activeAppsIsOpen = !_activeAppsIsOpen;

            if (_activeAppsIsOpen)
            {
                _notificationsIsOpen = false;
                _userLinksIsOpen = false;
            }
        }

        public void ToggleNotifications()
        {
            _notificationsIsOpen = !_notificationsIsOpen;

            if (_notificationsIsOpen)
            {
                _activeAppsIsOpen = false;
                _userLinksIsOpen = false;
            }
        }

        public void ToggleUserLinks()
        {
            _userLinksIsOpen = !_userLinksIsOpen;

            if (_userLinksIsOpen)
            {
                _activeAppsIsOpen = false;
                _notificationsIsOpen = false;
            }
        }
    }
}
