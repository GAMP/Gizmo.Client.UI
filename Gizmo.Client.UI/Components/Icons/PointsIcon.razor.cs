using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class PointsIcon : ComponentBase
    {
        public string FilterId { get; set; } = ComponentIdGenerator.Generate();

        public string Linear0Id { get; set; } = ComponentIdGenerator.Generate();

        public string Linear1Id { get; set; } = ComponentIdGenerator.Generate();
    }
}
