using Gizmo.Client.UI.View.Services;
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
        }

        private bool _isOpen { get; set; }

        #region PROPERTIES

        [Inject]
        NotificationsService NotificationsService { get; set; }

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

        #endregion

        protected override void OnInitialized()
        {
            this.SubscribeChange(NotificationsService.ViewState);
            base.OnInitialized();
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