using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [DefaultRoute(ClientRoutes.UserProfileRoute, DefaultRouteMatch = NavlinkMatch.Prefix)]
    [Route(ClientRoutes.UserProfileRoute)]
    public partial class Profile : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserProfileViewService UserService { get; set; }

        [Inject]
        UserChangeProfileViewService UserChangeProfileViewStateService { get; set; }

        [Inject]
        UserChangePasswordViewService UserChangePasswordViewStateService { get; set; }

        [Inject]
        UserProfileViewState ViewState { get; set; }

        private async Task OnClickUpdateProfileButtonHandler()
        {
            await UserChangeProfileViewStateService.StartAsync();
        }

        private Task OnClickUpdateEmailButtonHandler()
        {
            //var s = await DialogService.ShowChangeEmailDialogAsync();
            //if (s.Result == DialogAddResult.Success)
            //{
            //    try
            //    {
            //        var result = await s.WaitForDialogResultAsync();
            //    }
            //    catch (OperationCanceledException)
            //    {
            //    }
            //}

            return Task.CompletedTask;
        }

        private Task OnClickUpdateMobileButtonHandler()
        {
            //var s = await DialogService.ShowChangeMobileDialogAsync();
            //if (s.Result == DialogAddResult.Success)
            //{
            //    try
            //    {
            //        var result = await s.WaitForDialogResultAsync();
            //    }
            //    catch (OperationCanceledException)
            //    {
            //    }
            //}

            return Task.CompletedTask;
        }

        private async Task OnClickChangePasswordButtonHandler()
        {
            await UserChangePasswordViewStateService.StartAsync(true);
        }

        private Task OnClickChangePictureButtonHandler()
        {
            //var s = await DialogService.ShowChangePictureDialogAsync();
            //if (s.Result == DialogAddResult.Success)
            //{
            //    try
            //    {
            //        var result = await s.WaitForDialogResultAsync();
            //    }
            //    catch (OperationCanceledException)
            //    {
            //    }
            //}

            return Task.CompletedTask;
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
