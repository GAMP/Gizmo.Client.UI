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

        private bool _isOpen;

        [Inject]
        public AppExeViewState AppExeViewState
        {
            get { return _appExeViewState; }
            private set { _appExeViewState = value; }
        }

        [Inject]
        public AppExeExecutionViewState AppExeExecutionViewState
        {
            get { return _appExeExecutionViewState; }
            private set { _appExeExecutionViewState = value; }
        }

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

        private Task OnClickMainButtonHandler()
        {
            if (_appExeExecutionViewState.IsActive)
            {
                return AppExecutionService.AppExeAbortAsync(_appExeExecutionViewState.AppExeId);
            }
            else if (_appExeExecutionViewState.IsRunning && !_appExeViewState.Options.HasFlag(ExecutableOptionType.MultiRun))
            {
                return AppExecutionService.AppExeTerminateAsync(_appExeExecutionViewState.AppExeId);
            }
            else
            {
                return AppExecutionService.AppExeExecuteAsync(_appExeExecutionViewState.AppExeId, false, default);
            }
        }

        private Task OnAutoStartChange(bool isChecked)
        {
            return AppExecutionService.SetAutoLaunchAsync(_appExeExecutionViewState.AppExeId, isChecked);
        }

        private Task OnRepairClick()
        {
            _isOpen = false;
            return AppExecutionService.AppExeExecuteAsync(_appExeExecutionViewState.AppExeId, true, default);
        }

        private Task OnTerminateClick()
        {
            _isOpen = false;
            return AppExecutionService.AppExeTerminateAsync(_appExeExecutionViewState.AppExeId);
        }

        private void OnClickPersonalFileButtonHandler()
        {
            _isOpen = false;
        }

        protected override async Task OnInitializedAsync()
        {
            _appExeViewState = await AppExeViewStateLookupService.GetStateAsync(ExecutableId);
            _appExeExecutionViewState = await AppExeExecutionViewStateLookupService.GetStateAsync(ExecutableId);

            if (_appExeViewState != null)
            {
                this.SubscribeChange(_appExeViewState);
            }

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

            if (_appExeViewState != null)
            {
                this.UnsubscribeChange(_appExeViewState);
            }

            base.Dispose();
        }
    }
}
