using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class ClientTabItem : CustomDOMComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        private async Task OnClickHandler(MouseEventArgs args)
        {
            await OnClick.InvokeAsync(args);
            await InvokeVoidAsync("tabItemBringIntoView", Ref);
        }
    }
}
