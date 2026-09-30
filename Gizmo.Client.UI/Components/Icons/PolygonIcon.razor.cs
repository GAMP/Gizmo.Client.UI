using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class PolygonIcon : ComponentBase
    {
        public string FilterId { get; set; } = ComponentIdGenerator.Generate();

        public string Clip0rId { get; set; } = ComponentIdGenerator.Generate();

        public string Clip1Id { get; set; } = ComponentIdGenerator.Generate();
    }
}
