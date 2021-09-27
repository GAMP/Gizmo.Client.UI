using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuNotificationCard : CustomDOMComponentBase
    {
        [Parameter]
        public MenuNotificationViewModel Notification { get; set; }
    }
}