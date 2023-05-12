using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class UniversalExecutable : CustomDOMComponentBase
    {
        #region FIELDS

        private AppExeViewState _appExeViewState;
        private AppViewState _appViewState;
        private AppExeExecutionViewState _appExeExecutionViewState;

        #endregion

        [Inject]
        public AppExeViewStateLookupService AppExeViewStateLookupService { get; set; }

        [Inject]
        public AppViewStateLookupService AppViewStateLookupService { get; set; }

        [Inject]
        public AppExeExecutionViewStateLookupService AppExeExecutionViewStateLookupService { get; set; }

        [Inject]
        public AppExecutionService AppExecutionService { get; set; }

        [Parameter]
        public int ExecutableId { get; set; }

        [Parameter]
        public bool ShowProgressBar { get; set; }

        [Parameter]
        public bool ShowAppName { get; set; }

        [Parameter]
        public bool ShowExeName { get; set; }

        public Task OnClickHandler(MouseEventArgs args)
        {
            return AppExecutionService.AppExeExecuteAsync(_appExeExecutionViewState.AppExeId, false, default);
        }

        #region OVERRIDE

        protected override async Task OnInitializedAsync()
        {
            _appExeViewState = await AppExeViewStateLookupService.GetStateAsync(ExecutableId);
            _appViewState = await AppViewStateLookupService.GetStateAsync(_appExeViewState.ApplicationId);
            _appExeExecutionViewState = await AppExeExecutionViewStateLookupService.GetStateAsync(ExecutableId);

            this.SubscribeChange(_appExeViewState);
            this.SubscribeChange(_appViewState);
            this.SubscribeChange(_appExeExecutionViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_appExeExecutionViewState);
            this.UnsubscribeChange(_appViewState);
            this.UnsubscribeChange(_appExeViewState);

            base.Dispose();
        }

        #endregion
    }
}
