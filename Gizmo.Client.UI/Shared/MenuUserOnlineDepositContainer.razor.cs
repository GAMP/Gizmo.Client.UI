using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI
{
    public partial class MenuUserOnlineDepositContainer : CustomDOMComponentBase
    {
        private bool _isOpen;

        protected bool _shouldRender;

        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserOnlineDepositViewService UserOnlineDepositViewStateService { get; set; }

        [Inject]
        UserOnlineDepositViewState ViewState { get; set; }

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

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        private async Task PayFromPC()
        {
            if (ViewState.PaymentUrl is not null)
                await JSRuntime.InvokeVoidAsync("open", ViewState.PaymentUrl);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

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

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterPopup", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}
