using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class CartDialogResult : ComponentBase
    {
        [Parameter]
        public bool Ok { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Note { get; set; }

        [Parameter]
        public string Error { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }
    }
}
