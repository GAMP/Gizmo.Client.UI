using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableSelectorDialogApplication : CustomDOMComponentBase
    {
        private AppViewState _appViewState;

        [Inject]
        AppViewStateLookupService AppViewStateLookupService { get; set; }

        [Parameter]
        public int ApplicationId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _appViewState = await AppViewStateLookupService.GetStateAsync(ApplicationId);
            this.SubscribeChange(_appViewState);
            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_appViewState);
            base.Dispose();
        }
    }
}
