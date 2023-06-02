using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class GizNotification : CustomDOMComponentBase
    {
        [Parameter]
        public int Index { get; set; }

        [Parameter]
        public AlertDialogIcons Icon { get; set; }

        [Parameter]
        public EventCallback<int> OnClose { get; set; }

        private async Task CloseNotification()
        {
            await OnClose.InvokeAsync(Index);
        }
    }
}
