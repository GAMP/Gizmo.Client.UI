using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI
{
    public partial class UserActionsBar : CustomDOMComponentBase
    {
        [Inject]
        public UserMenuViewService UserMenuViewService { get; set; }

        [Inject]
        public AssistanceRequestViewState AssistanceRequestViewState { get; set; }

        [Inject]
        public UserOnlineDepositViewState UserOnlineDepositViewState { get; set; }

        [Inject]
        public IClientDialogService DialogService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(AssistanceRequestViewState);
            this.SubscribeChange(UserOnlineDepositViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(AssistanceRequestViewState);
            this.UnsubscribeChange(UserOnlineDepositViewState);
            base.Dispose();
        }

        private async Task ShowOnlineDeposits()
        {
            var dialog = await DialogService.ShowUserOnlineDepositsDialogAsync();
            if (dialog.Result == AddComponentResultCode.Opened)
                _ = await dialog.WaitForResultAsync();
        }
    }
}
