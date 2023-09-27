using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenu : CustomDOMComponentBase
    {
        [Inject]
        IClientDialogService DialogService { get; set; }

        [Inject]
        public UserMenuViewService UserMenuViewService { get; set; }

        [Inject]
        public UserOnlineDepositViewState UserOnlineDepositViewState { get; set; }

        private async Task ShowOnlineDeposits()
        {
            //UserMenuViewService.ToggleUserOnlineDeposit();

            //Open Dialog
            var dialog = await DialogService.ShowUserOnlineDepositsDialogAsync();
            if (dialog.Result == AddComponentResultCode.Opened)
                _ = await dialog.WaitForResultAsync();
        }
    }
}
