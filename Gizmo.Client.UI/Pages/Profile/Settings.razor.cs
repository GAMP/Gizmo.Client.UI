using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [DefaultRoute(ClientRoutes.UserSettingsRoute, DefaultRouteMatch = NavlinkMatch.Prefix)]
    [Route(ClientRoutes.UserSettingsRoute)]
    public partial class Settings : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserSettingsService UserSettingsService { get; set; }

        [Inject]
        UserSettingsViewState ViewState { get; set; }

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

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
