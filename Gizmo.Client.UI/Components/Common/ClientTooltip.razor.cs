using System;
using System.Threading.Tasks;
using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class ClientTooltip : CustomDOMComponentBase
    {
        const int OPEN_DEFAULT_DELAY = 500;
        const int CLOSE_DEFAULT_DELAY = 200;

        #region CONSTRUCTOR
        public ClientTooltip()
        {
            _openDeferredAction = new DeferredAction(Open);
            _openDelayTimeSpan = new TimeSpan(0, 0, 0, 0, _openDelay);

            _closeDeferredAction = new DeferredAction(Close);
            _closeDelayTimeSpan = new TimeSpan(0, 0, 0, 0, _closeDelay);
        }
        #endregion

        #region FIELDS

        private DeferredAction _openDeferredAction;
        private int _openDelay = OPEN_DEFAULT_DELAY;
        private TimeSpan _openDelayTimeSpan;

        private DeferredAction _closeDeferredAction;
        private int _closeDelay = CLOSE_DEFAULT_DELAY;
        private TimeSpan _closeDelayTimeSpan;

        private bool _isOpen;

        private bool _preventClose;

        protected bool _shouldRender;

        private ElementReference _tooltipContent;

        private double _popupX;
        private double _popupY;

        #endregion

        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment TooltipContent { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public TooltipOpenDirections OpenDirection { get; set; } = TooltipOpenDirections.Top;

        #endregion

        public void OnMouseOverHandler(MouseEventArgs args)
        {
            //_popupX = args.ClientX;
            //_popupY = args.ClientY;

            _preventClose = true;

            _openDeferredAction.Defer(_openDelayTimeSpan);
        }

        public void OnMouseOutHandler(MouseEventArgs args)
        {
            _preventClose = false;

            _closeDeferredAction.Defer(_closeDelayTimeSpan);
        }

        private async Task Open()
        {
            var windowSize = await JsInvokeAsync<WindowSize>("getWindowSize");
            var tooltipRootSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", Ref);
            var popupContentSize = await JsInvokeAsync<BoundingClientRect>("getElementBoundingClientRect", _tooltipContent);

            if (OpenDirection == TooltipOpenDirections.Top || OpenDirection == TooltipOpenDirections.Bottom)
            {
                _popupX = tooltipRootSize.Left + (tooltipRootSize.Width / 2) - (popupContentSize.Width / 2);

                if (OpenDirection == TooltipOpenDirections.Top)
                {
                    _popupY = (tooltipRootSize.Top - popupContentSize.Height) - 10; //TODO: AAA
                }
                else
                {
                    _popupY = (tooltipRootSize.Top + tooltipRootSize.Height) + 10; //TODO: AAA
                }
            }

            _isOpen = true;

            _shouldRender = true;

            await InvokeAsync(StateHasChanged);
        }

        private Task Close()
        {
            if (!_preventClose)
            {
                _openDeferredAction.Cancel();

                if (_isOpen)
                {
                    _isOpen = false;

                    _shouldRender = true;

                    return InvokeAsync(StateHasChanged);
                }
            }

            return Task.CompletedTask;
        }

        protected string ClassName => new ClassMapper()
                 .Add("giz-client-tooltip-root")
                 .AsString();

        protected string TooltipClassName => new ClassMapper()
                 .Add("giz-client-tooltip")
                 .Add($"giz-client-tooltip--{OpenDirection.ToDescriptionString()}")
                 .If($"giz-client-tooltip--open", () => _isOpen)
                 .AsString();

        protected string TooltipStyleValue => new StyleMapper()
                 .If($"top: {_popupY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => _isOpen)
                 .If($"left: {_popupX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px", () => _isOpen)
                 .AsString();
    }
}
