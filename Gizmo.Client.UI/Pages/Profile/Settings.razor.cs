using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [DefaultRoute("/settings", DefaultRouteMatch = NavlinkMatch.Prefix)]
    [Route("/settings")]
    public partial class Settings : ComponentBase
    {
        [Inject]
        UserSettingsService UserSettingsService { get; set; }

        [Inject]
        UserPasswordRecoveryMethodService UserPasswordRecoveryMethodService { get; set; }

        [Inject()]
        IClientDialogService DialogService { get; set; }

        private async Task OnClickUpdateEmailButtonHandler()
        {
            var s = await DialogService.ShowChangeEmailDialogAsync();
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private async Task OnClickUpdateMobileButtonHandler()
        {
            var s = await DialogService.ShowChangeMobileDialogAsync();
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private async Task OnClickChangePasswordButtonHandler()
        {
            var s = await DialogService.ShowChangePasswordDialogAsync();
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        private async Task OnClickChangePictureButtonHandler()
        {
            var s = await DialogService.ShowChangePictureDialogAsync();
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}
