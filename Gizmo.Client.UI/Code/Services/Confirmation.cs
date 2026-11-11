using System;
using System.Threading.Tasks;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    internal static class Confirmation
    {
        public static async Task<bool> AskAsync(IClientDialogService dialogs, ILogger logger, string title, string message)
        {
            try
            {
                var dialog = await dialogs.ShowAlertDialogAsync(title, message, AlertDialogButtons.YesNo);
                if (dialog.Result != AddComponentResultCode.Opened)
                    return false;

                var result = await dialog.WaitForResultAsync();

                return dialog.Result == AddComponentResultCode.Ok && result?.Button == AlertDialogResultButton.Yes;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Confirmation \"{Title}\" failed.", title);
                return false;
            }
        }
    }
}
