using System.Threading.Tasks;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class AlertDialog : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        [Parameter]
        public EventCallback<AlertDialogResult> ResultCallback { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Message { get; set; }

        [Parameter]
        public AlertDialogButtons Buttons { get; set; }

        [Parameter]
        public AlertDialogIcons Icon { get; set; }

        private async Task CloseDialog(AlertDialogResultButton alertDialogResultButton)
        {
            await ResultCallback.InvokeAsync(new AlertDialogResult() { Button = alertDialogResultButton });
        }
    }
}
