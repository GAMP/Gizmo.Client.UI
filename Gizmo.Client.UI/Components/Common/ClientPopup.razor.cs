using System;
using System.Threading.Tasks;
using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class ClientPopup : CustomDOMComponentBase, IAsyncDisposable
    {
        private bool _isOpen;

        private ElementReference _popupContent;

        private double _popupX;
        private double _popupY;

        private double _moveX = 0;

        private float _fontSize = 10;

        [Parameter]
        public RenderFragment Activator { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TooltipOpenDirections OpenDirection { get; set; } = TooltipOpenDirections.Top;

        private async void OnClickHandler(MouseEventArgs args)
        {
            if (!_isOpen)
                await Open();
        }

        private async Task Open()
        {
            _fontSize = await JsInvokeAsync<float>("getFontSize");
            var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");

            if (IsDisposed)
                return;

            var tooltipRootSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);
            var popupContentSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _popupContent);

            if (OpenDirection == TooltipOpenDirections.Top || OpenDirection == TooltipOpenDirections.Bottom)
            {
                _popupX = tooltipRootSize.Left + (tooltipRootSize.Width / 2) - (popupContentSize.Width / 2);

                if (OpenDirection == TooltipOpenDirections.Top)
                {
                    _popupY = (tooltipRootSize.Top - popupContentSize.Height) - _fontSize;
                }
                else
                {
                    _popupY = (tooltipRootSize.Top + tooltipRootSize.Height) + _fontSize;
                }
            }

            if (OpenDirection == TooltipOpenDirections.Left || OpenDirection == TooltipOpenDirections.Right)
            {
                _popupY = tooltipRootSize.Top + (tooltipRootSize.Height / 2) - (popupContentSize.Height / 2);

                if (OpenDirection == TooltipOpenDirections.Left)
                {
                    _popupX = (tooltipRootSize.Left - popupContentSize.Width) - _fontSize;
                }
                else
                {
                    _popupX = (tooltipRootSize.Left + tooltipRootSize.Width) + _fontSize;
                }
            }

            _moveX = 0;

            //Fix X position.
            if (_popupX < 0)
            {
                _moveX = (popupContentSize.Width / 2) + (_popupX);
                _popupX = 0;
            }

            if (_popupX + popupContentSize.Width > windowSize.Width)
            {
                _moveX = (popupContentSize.Width / 2) + (_popupX) - (windowSize.Width - popupContentSize.Width);
                _popupX = windowSize.Width - popupContentSize.Width;
            }

            _isOpen = true;

            //_shouldRender = true;

            await InvokeAsync(StateHasChanged);
        }

        public void Close()
        {
            _isOpen = false;

            StateHasChanged();
        }

        protected string PopupClassName => new ClassMapper()
                 .Add("giz-client-popup")
                 .Add($"giz-client-popup--{OpenDirection.ToDescriptionString()}")
                 .If($"open", () => _isOpen)
                 .AsString();

        protected string PopupStyleValue => new StyleMapper()
                 .If($"top: {_popupY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => _isOpen)
                 .If($"left: {_popupX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => _isOpen)
                 .AsString();

        protected string PopupPinStyleValue => new StyleMapper()
                 .If($"left: {_moveX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => _moveX != 0)
                 .AsString();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("registerPopup", _popupContent);
                ClosePopupEventInterop = new ClosePopupEventInterop(JsRuntime);
                await ClosePopupEventInterop.SetupClosePopupEventCallback(args => ClosePopupHandler(args));
            }
        }

        public override void Dispose()
        {
            ClosePopupEventInterop?.Dispose();

            base.Dispose();
        }

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private Task ClosePopupHandler(string args)
        {
            if (args == Id)
            {
                Close();
            }

            return Task.CompletedTask;
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterPopup", _popupContent).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}
