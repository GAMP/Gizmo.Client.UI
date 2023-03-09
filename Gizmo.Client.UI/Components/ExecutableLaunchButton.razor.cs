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

        [Parameter]
        public int ExecutableId { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public bool IsFullWidth { get; set; }

        private Task OnClickMainButtonHandler()
        {
            //switch (Executable.State)
            //{
            //	case View.ExecutableState.None:
            //		return ActiveApplicationsService.RunExecutableAsyc(Executable.ExecutableId);

            //	default:
            //		return ActiveApplicationsService.TerminateExecutableAsyc(Executable.ExecutableId);
            //}
            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            _appExeExecutionViewState = new AppExeExecutionViewState(); //TODO: AAA FIND EXECUTION STATE
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
