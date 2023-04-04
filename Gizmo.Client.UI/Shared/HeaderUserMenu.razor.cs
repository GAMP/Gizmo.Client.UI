using Gizmo.Web.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenu : CustomDOMComponentBase
    {
        private bool _topUpIsOpen;
        private bool _activeAppsIsOpen;
        private bool _notificationsIsOpen;
        private bool _userLinksIsOpen;

        public void ToggleTopUp()
        {
            _topUpIsOpen = !_topUpIsOpen;

            if (_topUpIsOpen)
            {
                _activeAppsIsOpen = false;
                _notificationsIsOpen = false;
                _userLinksIsOpen = false;
            }
        }

        public void ToggleActiveApps()
        {
            _activeAppsIsOpen = !_activeAppsIsOpen;

            if (_activeAppsIsOpen)
            {
                _topUpIsOpen = false;
                _notificationsIsOpen = false;
                _userLinksIsOpen = false;
            }
        }

        public void ToggleNotifications()
        {
            _notificationsIsOpen = !_notificationsIsOpen;

            if (_notificationsIsOpen)
            {
                _topUpIsOpen = false;
                _activeAppsIsOpen = false;
                _userLinksIsOpen = false;
            }
        }

        public void ToggleUserLinks()
        {
            _userLinksIsOpen = !_userLinksIsOpen;

            if (_userLinksIsOpen)
            {
                _topUpIsOpen = false;
                _activeAppsIsOpen = false;
                _notificationsIsOpen = false;
            }
        }
    }
}
