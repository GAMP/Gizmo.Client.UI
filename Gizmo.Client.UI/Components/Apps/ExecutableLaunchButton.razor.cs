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
        private AppExeViewState _appExeViewState;
        private AppExeExecutionViewState _appExeExecutionViewState;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject()]
        AppExecutionService AppExecutionService { get; set; }

        [Inject()]
        AppExeViewStateLookupService AppExeViewStateLookupService { get; set; }

        [Inject()]
        AppExeExecutionViewStateLookupService AppExeExecutionViewStateLookupService { get; set; }

        [Parameter]
        public int ExecutableId { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public bool IsFullWidth { get; set; }

        private async Task OnClickMainButtonHandler()
        {
            await AppExecutionService.AppExeExecuteAsync(_appExeExecutionViewState.AppExeId, default);
        }

        protected override async Task OnInitializedAsync()
        {
            _appExeViewState = await AppExeViewStateLookupService.GetStateAsync(ExecutableId);
            _appExeExecutionViewState = await AppExeExecutionViewStateLookupService.GetStateAsync(ExecutableId);

            this.SubscribeChange(_appExeViewState);
            this.SubscribeChange(_appExeExecutionViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_appExeExecutionViewState);
            this.UnsubscribeChange(_appExeViewState);

            base.Dispose();
        }
    }
}
