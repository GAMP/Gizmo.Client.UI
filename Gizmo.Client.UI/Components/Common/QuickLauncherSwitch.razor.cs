using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class QuickLauncherSwitch : CustomDOMComponentBase
    {
        private int _selectedTabIndex = 0;

        [Parameter]
        public int SelectedTabIndex
        {
            get
            {
                return _selectedTabIndex;
            }
            set
            {
                if (_selectedTabIndex == value)
                    return;

                _selectedTabIndex = value;
                _ = SelectedTabIndexChanged.InvokeAsync(_selectedTabIndex);
            }
        }

        [Parameter]
        public EventCallback<int> SelectedTabIndexChanged { get; set; }

        private void SelectTab(ICollection<Button> selectedItems)
        {
            if (selectedItems.Where(a => a.Name == "QuickLaunch").Any())
                SelectedTabIndex = 0;
            else
                SelectedTabIndex = 1;
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
    }
}
