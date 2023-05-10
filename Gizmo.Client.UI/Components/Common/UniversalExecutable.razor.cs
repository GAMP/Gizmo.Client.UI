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

        public AppExeViewState _appExeViewState;
        public AppViewState _appViewState;

        #endregion

        [Inject]
        public AppExeViewStateLookupService AppExeViewStateLookupService { get; set; }

        [Inject]
        public AppViewStateLookupService AppViewStateLookupService { get; set; }

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
            //switch (Executable.State)
            //{
            //    case View.ExecutableState.None:
            //        return ActiveApplicationsService.RunExecutableAsyc(Executable.ExecutableId);

            //    default:
            //        return ActiveApplicationsService.TerminateExecutableAsyc(Executable.ExecutableId);
            //}
            return Task.CompletedTask;
        }

        #region OVERRIDE

        protected override async Task OnInitializedAsync()
        {
            _appExeViewState = await AppExeViewStateLookupService.GetStateAsync(ExecutableId);
            _appViewState = await AppViewStateLookupService.GetStateAsync(_appExeViewState.ApplicationId);

            this.SubscribeChange(_appExeViewState);
            this.SubscribeChange(_appViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_appViewState);
            this.UnsubscribeChange(_appExeViewState);

            base.Dispose();
        }

        #endregion
    }
}
