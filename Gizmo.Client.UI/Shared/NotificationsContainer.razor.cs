using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Shared
{
    public partial class NotificationsContainer : CustomDOMComponentBase
    {
        public NotificationsContainer()
        {
            Notifications = Enumerable.Range(0, 8).Select(i => new NotificationViewModel()
            {
                Id = i,
                Title = $"Order on hold {i}",
                Message = "Your order is on hold. You will be further be notified once order is accepted."
            }).ToList();
        }

        public ICollection<NotificationViewModel> Notifications { get; set; }

        public void CloseDialog(int id)
        {
            var notificationToClose = Notifications.Where(a => a.Id == id).FirstOrDefault();
            if (notificationToClose != null)
            {
                Notifications.Remove(notificationToClose);
            }
        }
    }
}