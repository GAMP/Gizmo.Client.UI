using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public abstract class ProfileCardBase : CustomDOMComponentBase
    {
        protected bool IsInfoOpen { get; set; }

        protected async Task ToggleInfo(MouseEventArgs e)
        {
            if (IsInfoOpen)
            {
                IsInfoOpen = false;
                return;
            }

            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e);

            IsInfoOpen = true;
        }
    }
}
