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
        private AppExeViewState _appExeViewState;

        [Inject]
        AppExeViewStateLookupService AppExeViewStateLookupService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public int ExecutableId { get; set; }

        [Inject()]
        public AppExeViewState ViewState
        {
            get { return _appExeViewState;}
            private set { _appExeViewState = value; }
        }

        protected override async Task OnInitializedAsync()
        {
            _appExeViewState = await AppExeViewStateLookupService.GetStateAsync(ExecutableId);
            this.SubscribeChange(_appExeViewState);
            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_appExeViewState);
            base.Dispose();
        }
    }
}
