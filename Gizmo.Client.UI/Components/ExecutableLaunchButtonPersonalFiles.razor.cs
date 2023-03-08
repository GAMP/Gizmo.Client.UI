using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableLaunchButtonPersonalFiles : CustomDOMComponentBase
    {
        private int _previousExecutableId;
        private AppExeViewState _appExeViewState { get; set; }

        [Inject]
        AppExeViewStateLookupService AppExeViewStateLookupService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public int ExecutableId { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            var executableIdChanged = _previousExecutableId != ExecutableId;

            if (executableIdChanged)
            {
                if (_appExeViewState != null)
                {
                    //The same component used again with a different executable id.
                    //We have to unbind from the old executable.
                    this.UnsubscribeChange(_appExeViewState);
                }

                _previousExecutableId = ExecutableId;

                //We have to bind to the new executable.
                _appExeViewState = await AppExeViewStateLookupService.GetStateAsync(ExecutableId);
                this.SubscribeChange(_appExeViewState);
            }
        }

        public override void Dispose()
        {
            if (_appExeViewState != null)
            {
                this.UnsubscribeChange(_appExeViewState);
            }
            base.Dispose();
        }
    }
}
