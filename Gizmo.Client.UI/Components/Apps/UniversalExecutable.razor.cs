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
        public AppExeViewState AppExeViewState
        {
            get { return _appExeViewState; }
            private set { _appExeViewState = value; }
        }

        [Inject]
        public AppViewState AppViewState
        {
            get { return _appViewState; }
            private set { _appViewState = value; }
        }

        [Inject]
        public AppExeExecutionViewState AppExeExecutionViewState
        {
            get { return _appExeExecutionViewState; }
            private set { _appExeExecutionViewState = value; }
        }

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

            if (_appExeViewState != null)
            {
                this.SubscribeChange(_appExeViewState);
            }

            if (_appViewState != null)
            {
                this.SubscribeChange(_appViewState);
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

            if (_appViewState != null)
            {
                this.UnsubscribeChange(_appViewState);
            }

            if (_appExeViewState != null)
            {
                this.UnsubscribeChange(_appExeViewState);
            }

            base.Dispose();
        }

        #endregion
    }
}
