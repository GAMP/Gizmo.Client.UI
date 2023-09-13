using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenu : CustomDOMComponentBase
    {
        private bool _userOnlineDepositIsOpen;
        private bool _activeAppsIsOpen;
        private bool _notificationsIsOpen;
        private bool _userLinksIsOpen;
        private bool _assistanceIsOpen;

        [Inject]
        public UserOnlineDepositViewState UserOnlineDepositViewState { get; set; }

        public void ToggleUserOnlineDeposit()
        {
            _userOnlineDepositIsOpen = !_userOnlineDepositIsOpen;

            if (_userOnlineDepositIsOpen)
            {
                _activeAppsIsOpen = false;
                _notificationsIsOpen = false;
                _userLinksIsOpen = false;
                _assistanceIsOpen = false;
            }
        }

        public void ToggleActiveApps()
        {
            _activeAppsIsOpen = !_activeAppsIsOpen;

            if (_activeAppsIsOpen)
            {
                _userOnlineDepositIsOpen = false;
                _notificationsIsOpen = false;
                _userLinksIsOpen = false;
                _assistanceIsOpen = false;
            }
        }

        public void ToggleNotifications()
        {
            _notificationsIsOpen = !_notificationsIsOpen;

            if (_notificationsIsOpen)
            {
                _userOnlineDepositIsOpen = false;
                _activeAppsIsOpen = false;
                _userLinksIsOpen = false;
                _assistanceIsOpen = false;
            }
        }

        public void ToggleUserLinks()
        {
            _userLinksIsOpen = !_userLinksIsOpen;

            if (_userLinksIsOpen)
            {
                _userOnlineDepositIsOpen = false;
                _activeAppsIsOpen = false;
                _notificationsIsOpen = false;
                _assistanceIsOpen = false;
            }
        }

        public void ToggleAssistance()
        {
            _assistanceIsOpen = !_assistanceIsOpen;

            if (_assistanceIsOpen)
            {
                _userOnlineDepositIsOpen = false;
                _activeAppsIsOpen = false;
                _notificationsIsOpen = false;
                _userLinksIsOpen = false;
            }
        }
    }
}
