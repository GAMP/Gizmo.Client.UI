using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenu : CustomDOMComponentBase
    {
        public HeaderUserMenu()
        {
        }

        private bool _activeAppsIsOpen;
        private bool _notificationsIsOpen;
        private bool _userLinksIsOpen;

        [Inject]
        UserService UserService { get; set; }

        [Inject]
        NotificationsService NotificationsService { get; set; }

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

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserService.ViewState);
            this.SubscribeChange(NotificationsService.ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(NotificationsService.ViewState);
            this.UnsubscribeChange(UserService.ViewState);

            base.Dispose();
        }

    }
}