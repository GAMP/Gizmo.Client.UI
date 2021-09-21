using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class NotificationViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}