using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    /// <summary>
    /// Base for profile grid cards with a hover-revealed (i) that opens a <see cref="ProfileCardPopup"/>.
    /// </summary>
    public abstract class ProfileCardBase : CustomDOMComponentBase
    {
        /// <summary>True while this card's info popup is open (bound to the popup shell).</summary>
        protected bool IsInfoOpen { get; set; }

        protected async Task ToggleInfo(MouseEventArgs e)
        {
            if (IsInfoOpen)
            {
                IsInfoOpen = false;
                return;
            }

            // the icon click stops propagation, so the layout's outside-click handler never sees it;
            // close any other open popup here before opening ours (ours is not registered as open yet)
            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e);

            IsInfoOpen = true;
        }
    }
}
