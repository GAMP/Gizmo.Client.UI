using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class RssFeedItemViewModel : ViewModelBase
    {
        public string Source { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
