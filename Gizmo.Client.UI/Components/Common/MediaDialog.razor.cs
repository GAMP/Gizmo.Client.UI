using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class MediaDialog : CustomDOMComponentBase
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public AdvertisementMediaUrlType MediaUrlType { get; set; }

        [Parameter]
        public string MediaUrl { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }
    }
}
