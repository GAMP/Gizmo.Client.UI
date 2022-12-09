using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizDockItem : CustomDOMComponentBase
    {
        const int OPEN_DEFAULT_DELAY = 2000;
        const int CLOSE_DEFAULT_DELAY = 200;

        #region CONSTRUCTOR
        public GizDockItem()
        {
            _openDeferredAction = new DeferredAction(Open);
            _openDelayTimeSpan = new TimeSpan(0, 0, 0, 0, _openDelay);

            _closeDeferredAction = new DeferredAction(Close);
            _closeDelayTimeSpan = new TimeSpan(0, 0, 0, 0, _closeDelay);
        }
        #endregion

        #region FIELDS

        private bool _scaled;
        private bool _halfScaled;

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

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public void OnMouseOverHandler(MouseEventArgs args)
        {
            _preventClose = true;

            _openDeferredAction.Defer(_openDelayTimeSpan);

            InvokeVoidAsync("writeLine", $"OnMouseOverHandler {this.ToString()}");
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

        private Task Open()
        {
            _isOpen = true;

            _shouldRender = true;

            return InvokeAsync(StateHasChanged);
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

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                .Add("giz-dock-item")
                .AsString();

        #endregion
    }
}