using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class QRIcon : ComponentBase
    {
        public string FilterId { get; set; } = ComponentIdGenerator.Generate();
    }
}
