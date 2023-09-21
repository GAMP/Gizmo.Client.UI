using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizDockItem : CustomDOMComponentBase
    {
        const int OPEN_DEFAULT_DELAY = 500;
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

        private DeferredAction _openDeferredAction;
        private int _openDelay = OPEN_DEFAULT_DELAY;
        private TimeSpan _openDelayTimeSpan;

        private DeferredAction _closeDeferredAction;
        private int _closeDelay = CLOSE_DEFAULT_DELAY;
        private TimeSpan _closeDelayTimeSpan;

        private bool _isOpen;

        private bool _preventClose;

        private AppExeViewState _previousExecutable;

        #endregion

        [Inject]
        public ActiveApplicationsViewService ActiveApplicationsService { get; set; }

        [Parameter]
        public AppExeViewState Executable { get; set; }

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

        private Task Open()
        {
            _isOpen = true;

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

                    return InvokeAsync(StateHasChanged);
                }
            }

            return Task.CompletedTask;
        }

        #region OVERRIDE

        protected override async Task OnParametersSetAsync()
        {
            bool executableChanged = !EqualityComparer<AppExeViewState>.Default.Equals(_previousExecutable, Executable);

            if (executableChanged)
            {
                if (_previousExecutable != null)
                {
                    //Remove handler
                    this.UnsubscribeChange(_previousExecutable);
                }
                if (Executable != null)
                {
                    //Add handler
                    this.SubscribeChange(Executable);
                }
            }

            _previousExecutable = Executable;

            await base.OnParametersSetAsync();
        }

        public override void Dispose()
        {
            if (Executable != null)
            {
                this.UnsubscribeChange(Executable);
            }

            base.Dispose();
        }

        #endregion

    }
}
