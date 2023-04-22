using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class NotificationDialog : CustomDOMComponentBase
    {
        [Inject]
        NotificationsViewService NotificationsService { get; set; }

        [Parameter]
        public NotificationViewState Notification { get; set; }
    }
}