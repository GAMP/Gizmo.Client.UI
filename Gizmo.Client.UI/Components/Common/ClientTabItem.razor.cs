using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ClientTabItem : CustomDOMComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private async void OnClickHandler()
        {
            await InvokeVoidAsync("tabItemBringIntoView", Ref);
        }
    }
}
