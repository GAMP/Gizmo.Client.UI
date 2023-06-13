using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class QuickLauncher : CustomDOMComponentBase
    {
        public QuickLauncher()
        {
        }

        [Inject]
        UserQuickLaunchViewService UserQuickLaunchService { get; set; }

        [Inject]
        UserQuickLaunchViewState ViewState { get; set; }

        private void SelectedTabIndexChanged(int selectedTabIndex)
        {
            UserQuickLaunchService.SetSelectedTabIndex(selectedTabIndex);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

			base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

			base.Dispose();
        }
    }
}
