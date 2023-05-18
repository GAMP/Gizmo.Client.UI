using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableLaunchButtonPersonalFiles : CustomDOMComponentBase
    {
        private AppExeViewState _appExeViewState;

        private bool _isExpanded;

        [Inject]
        public AppExeViewState AppExeViewState
        {
            get { return _appExeViewState; }
            private set { _appExeViewState = value; }
        }

        [Inject]
        AppExeViewStateLookupService AppExeViewStateLookupService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject()]
        public AppExeViewState ViewState
        {
            get { return _appExeViewState; }
            private set { _appExeViewState = value; }
        }

        [Parameter]
        public int ExecutableId { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        private async Task OnClickPersonalFileButtonHandler(MouseEventArgs args)
        {
            _isExpanded = false;
            await OnClick.InvokeAsync(args);
        }

        protected override async Task OnInitializedAsync()
        {
            _appExeViewState = await AppExeViewStateLookupService.GetStateAsync(ExecutableId);

            if (_appExeViewState != null)
            {
                this.SubscribeChange(_appExeViewState);
            }

            await base.OnInitializedAsync();
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
