using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserBalanceCurrentTimeProduct : CustomDOMComponentBase, IDisposable
    {
        const int OPEN_DEFAULT_DELAY = 500;
        const int CLOSE_DEFAULT_DELAY = 200;

        #region CONSTRUCTOR
        public HeaderUserBalanceCurrentTimeProduct()
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

        #endregion

        [Inject]
        UsageSessionViewState UsageSessionViewState { get; set; }

        [Inject]
        TimeProductsViewService TimeProductsViewService { get; set; }

        public void OnMouseOverHandler(MouseEventArgs args)
        {
            _preventClose = true;

            _openDeferredAction.Defer(_openDelayTimeSpan);
        }

        public void OnMouseOutHandler(MouseEventArgs args)
        {
            _preventClose = false;

            _closeDeferredAction.Defer(_closeDelayTimeSpan);
        }

        public void OnMouseOverTooltipHandler(MouseEventArgs args)
        {
            _preventClose = true;
        }

        public void OnMouseOutTooltipHandler(MouseEventArgs args)
        {
            _preventClose = false;

            _closeDeferredAction.Defer(_closeDelayTimeSpan);
        }

        private async Task Open()
        {
            await TimeProductsViewService.LoadAsync();

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

        #region OVERRIDE

        protected override Task OnInitializedAsync()
        {
            this.SubscribeChange(UsageSessionViewState);

            return base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UsageSessionViewState);

            base.Dispose();
        }

        #endregion
    }
}
