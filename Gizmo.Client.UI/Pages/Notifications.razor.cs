using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/notifications")]
    public partial class Notifications : ComponentBase
    {
        private bool _slideOut = false;
        private int _total = 1;
        private int _slideInIndex = -1;
        private int _slideOutIndex = -1;

        private void DemoAdd()
        {
            _slideOutIndex = -1;
            _slideInIndex = _total;
            _total += 1;
        }

        private void CloseNotifications()
        {
            _slideOut = true;
            //TODO: Allow rerender to change class and start animation
            //Wait enough to complete animation and then send to service that is closed.
        }

        private async Task OnCloseHandler(int index)
        {
            _slideOutIndex = index;
            _slideInIndex = -1;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(500);
            _total -= 1;
            _slideOutIndex = -1;
            await InvokeAsync(StateHasChanged);
        }
    }
}
