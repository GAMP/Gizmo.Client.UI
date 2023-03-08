﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableLaunchButton : CustomDOMComponentBase
    {
        private int _previousExecutableId;
        private AppExeExecutionViewState _appExeExecutionViewState { get; set; }

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
