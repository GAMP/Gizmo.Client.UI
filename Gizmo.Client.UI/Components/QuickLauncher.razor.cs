using Gizmo.Client.UI.View.Services;
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
        UserQuickLaunchService UserQuickLaunchService { get; set; }

        [Inject]
        QuickLaunchService QuickLaunchService { get; set; }

        [Inject]
        FavoritesService FavoritesService { get; set; }

        private void SelectedTabIndexChanged(int selectedTabIndex)
        {
            UserQuickLaunchService.SetSelectedTabIndex(selectedTabIndex);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserQuickLaunchService.ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserQuickLaunchService.ViewState);
            base.Dispose();
        }
    }
}