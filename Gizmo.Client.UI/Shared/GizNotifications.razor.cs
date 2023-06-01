using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class GizNotifications : CustomDOMComponentBase
    {
        [Parameter]
        public AlertDialogIcons Icon { get; set; }

        private void CloseNotification()
        {

        }
    }
}
