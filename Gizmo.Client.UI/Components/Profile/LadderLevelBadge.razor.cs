using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class LadderLevelBadge : CustomDOMComponentBase
    {
        [Parameter] public int Ordinal { get; set; }
        [Parameter] public string? EmblemUrl { get; set; }
    }
}
