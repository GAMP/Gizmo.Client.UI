using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuNotificationCard : CustomDOMComponentBase
    {
        [Parameter]
        public MenuNotificationViewModel Notification { get; set; }

        [Parameter]
        public EventCallback<int> OnMarkAsRead { get; set; }

        private async Task MarkAsRead()
        {
            await OnMarkAsRead.InvokeAsync(Notification.Id);
        }
    }
}