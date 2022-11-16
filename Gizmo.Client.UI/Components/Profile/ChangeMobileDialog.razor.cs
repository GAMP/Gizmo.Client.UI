using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangeMobileDialog : CustomDOMComponentBase
    {
        public ChangeMobileDialog()
        {
        }

        [Inject]
        UserChangeMobileService UserChangeMobileService { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }
        
        private async Task CloseDialog()
        {
            await CancelCallback.InvokeAsync();

            if (UserChangeMobileService.ViewState.IsComplete)
                await UserChangeMobileService.ResetAsync();
        }
    }
}