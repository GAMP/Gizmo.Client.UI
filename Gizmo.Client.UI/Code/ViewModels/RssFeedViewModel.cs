using System.Collections.Generic;

namespace Gizmo.Client.UI.ViewModels
{
    public class RssFeedViewModel : NewsFeedViewModel
    {
        public List<RssFeedItemViewModel> Items { get; set; }
    }
}
