using System.Collections.Generic;

namespace Gizmo.Client.UI.ViewModels
{
    public class RssFeed : NewsFeed
    {
        public List<RssFeedItemViewModel> Items { get; set; }
    }
}
