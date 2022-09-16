using Gizmo.Web.Components;
using System.Collections.Generic;
using System.Linq;

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
    }
}