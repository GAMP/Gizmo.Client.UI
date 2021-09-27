using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuNotificationsContainer : CustomDOMComponentBase
    {
        public MenuNotificationsContainer()
        {
            Notifications = new List<MenuNotificationViewModel>();

            Notifications.Add(new MenuNotificationViewModel() { Id = 1, Title = "New order #0075364 created.", Time = "1 hour ago" });
            Notifications.Add(new MenuNotificationViewModel() { Id = 2, Title = "Order #0075364 was successfuly paid from your account.", Time = "1 hour ago" });
            Notifications.Add(new MenuNotificationViewModel() { Id = 3, Title = "05h00m was added to your account.", Time = "2 days ago" });
            Notifications.Add(new MenuNotificationViewModel() { Id = 4, Title = "Order #0075364 was successfuly paid from your account.", Time = "3 days ago" });
        }

        public List<MenuNotificationViewModel> Notifications { get; set; }
    }
}
