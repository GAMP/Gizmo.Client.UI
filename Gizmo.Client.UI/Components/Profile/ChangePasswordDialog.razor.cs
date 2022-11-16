using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangePasswordDialog : CustomDOMComponentBase
    {
        public ChangePasswordDialog()
        {
        }

        [Inject]
        UserChangePasswordService UserChangePasswordService { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private async Task CloseDialog()
        {
            await CancelCallback.InvokeAsync();

            if (UserChangePasswordService.ViewState.IsComplete)
                await UserChangePasswordService.ResetAsync();
        }
    }
}