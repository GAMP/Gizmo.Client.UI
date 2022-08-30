using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class AdsCarouselItem : CustomDOMComponentBase
    {
        [Parameter]
        public AdvertisementViewState Advertisement { get; set; }
    }
}
