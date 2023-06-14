using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuNotificationsContainer : CustomDOMComponentBase, IAsyncDisposable
    {        
        public MenuNotificationsContainer()
        {
        }

        private bool _isOpen;

        protected bool _shouldRender;

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        INotificationsService NotificationsService { get; set; }

        [Inject]
        NotificationsHostViewState ViewState { get; set; }

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
            this.SubscribeChange(ViewState); //TODO: A WE NEED TO UPDATE _shouldRender FROM SubscribeChange.

            base.OnInitialized();
        }

        protected override bool ShouldRender()
        {
            return true; //TODO: A _shouldRender
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<bool>(nameof(IsOpen), out var newIsOpen))
            {
                var isOpenChanged = IsOpen != newIsOpen;
                if (isOpenChanged)
                {
                    _shouldRender = true;
                }
            }

            await base.SetParametersAsync(parameters);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                await JsRuntime.InvokeVoidAsync("registerPopup", Ref);
                ClosePopupEventInterop = new ClosePopupEventInterop(JsRuntime);
                await ClosePopupEventInterop.SetupClosePopupEventCallback(args => ClosePopupHandler(args));
            }
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            ClosePopupEventInterop?.Dispose();

            base.Dispose();
        }

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private Task ClosePopupHandler(string args)
        {
            if (args == Id)
            {
                _shouldRender = true;

                IsOpen = false;
            }

            return Task.CompletedTask;
        }

        private void AcknowledgeAll()
        {
            IsOpen = false;
            NotificationsService.AcknowledgeAll();
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterPopup", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}
