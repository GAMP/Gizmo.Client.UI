using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ClientTab : CustomDOMComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
