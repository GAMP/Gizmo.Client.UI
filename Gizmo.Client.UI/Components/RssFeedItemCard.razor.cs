using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class RssFeedItemCard
    {
        [Parameter]
        public RssFeedItemViewModel RssFeedItem { get; set; }
    }
}
