using System.Collections.Generic;

using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class PayWayChooser : ComponentBase
    {
        [Parameter]
        public string Caption { get; set; }

        [Parameter]
        public IReadOnlyList<PaySegment> Segments { get; set; }

        [Parameter]
        public IEnumerable<PayWay> Chips { get; set; } = System.Array.Empty<PayWay>();

        [Parameter]
        public bool ShowChips { get; set; }

        [Parameter]
        public PayWay Selected { get; set; }

        [Parameter]
        public PayWay Single { get; set; }

        [Parameter]
        public string SingleNote { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public EventCallback<PaySegment> OnSegment { get; set; }

        [Parameter]
        public EventCallback<PayWay> OnWay { get; set; }
    }
}
