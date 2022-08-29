using Gizmo.Web.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class QuickLauncherSwitch : CustomDOMComponentBase
    {
        private int _selectedTabIndex = 0;

        private void SelectTab(int index)
        {
            _selectedTabIndex = index;
        }
    }
}