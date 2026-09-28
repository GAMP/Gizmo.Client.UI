using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    /// <summary>
    /// Square achievement image tile with the completions counter pill centered on its top edge.
    /// Parameter-only: no view state or client services.
    /// The fallback (no image) is a stand-in until design provides the empty variant.
    /// </summary>
    public partial class AchievementTile : CustomDOMComponentBase
    {
        [Parameter] public string? ImageUrl { get; set; }
        [Parameter] public string CountText { get; set; } = string.Empty;
    }
}
