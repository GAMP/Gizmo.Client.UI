using Gizmo.Web.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class Header : CustomDOMComponentBase
    {
        private bool _isOpen;

        public void ToggleActiveApps()
        {
            _isOpen = !_isOpen;
        }
    }
}
