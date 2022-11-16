using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangeEmailDialog : CustomDOMComponentBase
    {
        public ChangeEmailDialog()
        {
        }

        [Inject]
        UserChangeEmailService UserChangeEmailService { get; set; }


        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private async Task CloseDialog()
        {
            await CancelCallback.InvokeAsync();

            if (UserChangeEmailService.ViewState.IsComplete)
                await UserChangeEmailService.ResetAsync();
        }
    }
}