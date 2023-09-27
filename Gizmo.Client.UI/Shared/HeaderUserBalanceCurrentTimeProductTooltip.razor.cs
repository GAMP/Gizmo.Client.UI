using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class HeaderUserBalanceCurrentTimeProductTooltip : CustomDOMComponentBase
    {
        [Inject]
        IClientDialogService DialogService { get; set; }

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
        UserMenuViewService UserMenuViewService { get; set; }

        public void OpenDetails()
        {
            NavigationService.NavigateTo(ClientRoutes.UserProductsRoute);
        }

        public void OpenShop()
        {
            NavigationService.NavigateTo(ClientRoutes.ShopRoute);
        }
        
        private async Task OpenUserOnlineDeposit()
        {
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

            return base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(TimeProductsViewState);

            base.Dispose();
        }

        #endregion
    }
}
