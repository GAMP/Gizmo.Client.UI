using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuNotificationsContainer : CustomDOMComponentBase
    {
        public MenuNotificationsContainer()
        {
            Notifications = new List<MenuNotificationViewModel>();

            Notifications.Add(new MenuNotificationViewModel() { Id = 1, Title = "New order #0075364 created.", Time = "1 hour ago" });
            Notifications.Add(new MenuNotificationViewModel() { Id = 2, Title = "Order #0075364 was successfuly paid from your account.", Time = "1 hour ago" });
            Notifications.Add(new MenuNotificationViewModel() { Id = 3, Title = "05h00m was added to your account.", Time = "2 days ago" });
            Notifications.Add(new MenuNotificationViewModel() { Id = 4, Title = "Order #0075364 was successfuly paid from your account.", Time = "3 days ago" });
        }

        private bool _isOpen { get; set; }

        #region PROPERTIES

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;

                _ = IsOpenChanged.InvokeAsync(_isOpen);
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        public List<MenuNotificationViewModel> Notifications { get; set; }

        #endregion

        #region METHODS

        private void MarkAllAsRead()
        {
            Notifications.Clear();
        }

        private void MarkAsRead(int id)
        {
            var existingNotification = Notifications.Where(a => a.Id == id).FirstOrDefault();
            if (existingNotification != null)
            {
                Notifications.Remove(existingNotification);
            }
        }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //await JsRuntime.InvokeVoidAsync("registerPopup", Ref);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("registerPopup", Ref);
                ClosePopupEventInterop = new ClosePopupEventInterop(JsRuntime);
                await ClosePopupEventInterop.SetupClosePopupEventCallback(args => ClosePopupHandler(args));
            }
        }

        public override void Dispose()
        {
            ClosePopupEventInterop?.Dispose();
            _ = JsRuntime.InvokeVoidAsync("unregisterPopup", Ref);

            base.Dispose();
        }

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private Task ClosePopupHandler(string args)
        {
            if (args == Id)
                IsOpen = false;

            return Task.CompletedTask;
        }

    }
}