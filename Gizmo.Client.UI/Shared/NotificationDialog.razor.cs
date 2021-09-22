using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class NotificationDialog : CustomDOMComponentBase
    {
        [Parameter]
        public EventCallback<int> OnCloseDialog { get; set; }

        [Parameter]
        public NotificationViewModel Notification { get; set; }

        public async Task CloseDialog()
        {
            await OnCloseDialog.InvokeAsync(Notification.Id);
        }
    }
}