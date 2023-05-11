using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableLaunchButton : CustomDOMComponentBase
    {
        private AppExeExecutionViewState _appExeExecutionViewState;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject()]
        AppExecutionService AppExecutionService { get; set; }

        [Inject()]
        AppExeExecutionViewStateLookupService AppExeExecutionViewStateLookupService { get; set; }

        [Inject()]
        AppExeExecutionViewState ViewState
        {
            get { return _appExeExecutionViewState; }
            set { _appExeExecutionViewState = value; }
        }
        

        [Parameter]
        public int ExecutableId { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public bool IsFullWidth { get; set; }

        private async Task OnClickMainButtonHandler()
        {
            await AppExecutionService.AppExeExecuteAsync(_appExeExecutionViewState.AppExeId,default);
        }

        protected override async Task OnInitializedAsync()
        {
            _appExeExecutionViewState = await AppExeExecutionViewStateLookupService.GetStateAsync(ExecutableId);
            this.SubscribeChange(_appExeExecutionViewState);
            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_appExeExecutionViewState);
            base.Dispose();
        }
    }
}
