using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class GizDockItemExecutionState : CustomDOMComponentBase
    {
        private int _previousExecutableId;
        private AppExeExecutionViewState _appExeExecutionViewState { get; set; }

        [Parameter]
        public int ExecutableId { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            var executableIdChanged = _previousExecutableId != ExecutableId;

            if (executableIdChanged)
            {
                if (_appExeExecutionViewState != null)
                {
                    //The same component used again with a different executable id.
                    //We have to unbind from the old executable.
                    this.UnsubscribeChange(_appExeExecutionViewState);
                }

                _previousExecutableId = ExecutableId;

                //We have to bind to the new executable.
                _appExeExecutionViewState = new AppExeExecutionViewState(); //TODO: AAA FIND EXECUTION STATE
                this.SubscribeChange(_appExeExecutionViewState);
            }
        }

        public override void Dispose()
        {
            if (_appExeExecutionViewState != null)
            {
                this.UnsubscribeChange(_appExeExecutionViewState);
            }
            base.Dispose();
        }
    }
}
