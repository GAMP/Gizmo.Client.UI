using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class AchievementCard : CustomDOMComponentBase
    {
        private bool _isOpen;

        [Parameter] public UserAchievementViewState Item { get; set; } = null!;

        private async Task ToggleInfo(MouseEventArgs e)
        {
            if (_isOpen)
            {
                _isOpen = false;
                return;
            }

            // the icon click stops propagation, so the layout's outside-click handler never sees it;
            // close any other open popup here before opening ours (ours is not registered as open yet)
            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e);

            _isOpen = true;
        }
    }
}
