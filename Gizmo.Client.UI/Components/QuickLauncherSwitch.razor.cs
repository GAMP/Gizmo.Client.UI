using Gizmo.Web.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class QuickLauncherSwitch : CustomDOMComponentBase
    {
        private int _selectedTabIndex = 0;

        private void SelectTab(ICollection<Button> selectedItems)
        {
            if (selectedItems.Where(a => a.Name == "QuickLaunch").Any())
                _selectedTabIndex = 0;
            else
                _selectedTabIndex = 1;
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
    }
}