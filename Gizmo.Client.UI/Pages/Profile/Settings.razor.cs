using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [DefaultRoute(ClientRoutes.UserSettingsRoute, DefaultRouteMatch = NavlinkMatch.Prefix)]
    [Route(ClientRoutes.UserSettingsRoute)]
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

        protected override async Task OnInitializedAsync()
        {
            await UserSettingsService.LoadAsync();

            await base.OnInitializedAsync();
        }
    }
}
