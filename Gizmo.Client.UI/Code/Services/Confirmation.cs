using System.Threading.Tasks;
using Gizmo.UI;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    internal static class Confirmation
    {
        public static async Task<bool> AskAsync(IClientDialogService dialogs, string title, string message)
        {
            var dialog = await dialogs.ShowAlertDialogAsync(title, message, AlertDialogButtons.YesNo);
            if (dialog.Result != AddComponentResultCode.Opened)
                return false;

            var result = await dialog.WaitForResultAsync();

            return dialog.Result == AddComponentResultCode.Ok && result?.Button == AlertDialogResultButton.Yes;
        }
    }
}
