using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class HeaderUserBalanceCurrentTimeProductTooltip : CustomDOMComponentBase
    {
        /// <summary>
        /// Remaining time at which the tooltip starts urging: enough to top up or buy a
        /// package before the session ends, not so much that it nags all evening.
        /// </summary>
        private static readonly System.TimeSpan LOW_TIME = System.TimeSpan.FromMinutes(15);

        [Inject]
        IClientDialogService DialogService { get; set; }

        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        TimeProductsViewState TimeProductsViewState { get; set; }

        [Inject]
        CreditOptionsViewState CreditOptionsViewState { get; set; }

        [Inject]
        UserBalanceTooltipViewState ViewState { get; set; }

        [Inject]
        UserOnlineDepositViewState UserOnlineDepositViewState { get; set; }

        [Inject]
        UserMenuViewService UserMenuViewService { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickAction { get; set; }

        public async Task OpenDetails(MouseEventArgs args)
        {
            await OnClickAction.InvokeAsync(args);

            NavigationService.NavigateTo(ClientRoutes.UserProductsRoute);
        }

        public async Task OpenShop(MouseEventArgs args)
        {
            await OnClickAction.InvokeAsync(args);

            NavigationService.NavigateTo(ClientRoutes.ShopRoute);
        }
        
        private async Task OpenUserOnlineDeposit(MouseEventArgs args)
        {
            await OnClickAction.InvokeAsync(args);

            //Open Menu
            //UserMenuViewService.OpenUserOnlineDeposit();

            //Open Dialog
            var dialog = await DialogService.ShowUserOnlineDepositsDialogAsync();
            if (dialog.Result == AddComponentResultCode.Opened)
                _ = await dialog.WaitForResultAsync();
        }

        #region OVERRIDE

        protected override Task OnInitializedAsync()
        {
            this.SubscribeChange(TimeProductsViewState);
            this.SubscribeChange(UserBalanceViewState);

            return base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(TimeProductsViewState);
            this.UnsubscribeChange(UserBalanceViewState);

            base.Dispose();
        }

        #endregion
    }
}
