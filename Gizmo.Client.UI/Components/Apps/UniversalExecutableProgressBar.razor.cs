using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class UniversalExecutableProgressBar : CustomDOMComponentBase
    {
        private AppExeExecutionViewState _appExeExecutionViewState { get; set; }

        [Inject]
        public AppExeExecutionViewState AppExeExecutionViewState
        {
            get { return _appExeExecutionViewState; }
            private set { _appExeExecutionViewState = value; }
        }

        [Inject()]
        AppExeExecutionViewStateLookupService AppExeExecutionViewStateLookupService { get; set; }

        [Inject()]
        AppExeExecutionViewState ViewState
        {
            get
            {
                return _appExeExecutionViewState;
            }
            set
            {
                _appExeExecutionViewState = value;
            }
        }

        [Parameter]
        public int ExecutableId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _appExeExecutionViewState = await AppExeExecutionViewStateLookupService.GetStateAsync(ExecutableId);

            if (_appExeExecutionViewState != null)
            {
                this.SubscribeChange(_appExeExecutionViewState);
            }

            await base.OnInitializedAsync();
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
