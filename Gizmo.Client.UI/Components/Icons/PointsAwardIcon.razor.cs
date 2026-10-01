using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class PointsAwardIcon : ComponentBase
    {
        public string MaskId { get; set; } = ComponentIdGenerator.Generate();

        public string LinearGradientId { get; set; } = ComponentIdGenerator.Generate();

        public string ClipPathId { get; set; } = ComponentIdGenerator.Generate();
    }
}
