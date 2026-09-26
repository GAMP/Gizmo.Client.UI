using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class HeaderUserBalanceCurrentTimeProductTooltip : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

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
        UserOnlineDepositViewState UserOnlineDepositViewState { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickAction { get; set; }

        protected List<TimeProductViewState> ActiveProducts => TimeProductsViewState.IsInitialized == true
            ? TimeProductsViewState.TimeProducts.Where(product => product.ActivationOrder.HasValue).OrderBy(product => product.ActivationOrder).Take(2).ToList()
            : new List<TimeProductViewState>();

        protected TimeProductViewState SoleProduct
        {
            get
            {
                if (TimeProductsViewState.IsInitialized != true)
                    return null;

                var active = TimeProductsViewState.TimeProducts.Where(product => product.ActivationOrder.HasValue).ToList();

                return active.Count == 1 ? active[0] : null;
            }
        }

        protected bool CanTopUp => UserOnlineDepositViewState.IsEnabled;

        protected bool CanShop => !ViewState.DisableShop;

        protected bool HasActions => CanTopUp || CanShop;

        protected bool SessionWillNotExpire => SoleProduct is { } product && string.IsNullOrEmpty(product.UsableTime);

        protected bool SessionWillExpire => SoleProduct is { } product && !string.IsNullOrEmpty(product.UsableTime);

        protected bool ShowCreditNote => TimeProductsViewState.IsInitialized == true
            && SoleProduct is null
            && CreditOptionsViewState.TimeCreditType != CreditType.NoCredit
            && CreditOptionsViewState.IsUserTimeCreditEnabled;

        protected bool SoleProductInCredit => SoleProduct?.InCredit == true;

        protected string ContinueMessageKey
        {
            get
            {
                if (!HasActions)
                    return null;

                if (SoleProductInCredit)
                {
                    if (CanShop && CanTopUp)
                        return nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_MENU_BALANCE_TOOLTIP_TO_CONTINUE_CLOSE_BALANCE_AND_BUY_TIME_OR_TOP_UP);

                    return CanShop
                        ? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_MENU_BALANCE_TOOLTIP_TO_CONTINUE_CLOSE_BALANCE_AND_BUY_TIME)
                        : nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_MENU_BALANCE_TOOLTIP_TO_CONTINUE_CLOSE_BALANCE_AND_TOP_UP);
                }

                if (CanShop && CanTopUp)
                    return nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_MENU_BALANCE_TOOLTIP_TO_CONTINUE_BUY_TIME_OR_TOP_UP);

                return CanShop
                    ? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_MENU_BALANCE_TOOLTIP_TO_CONTINUE_BUY_TIME)
                    : nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_MENU_BALANCE_TOOLTIP_TO_CONTINUE_TOP_UP);
            }
        }

        protected string ExpireNoteClass => SoleProductInCredit ? "giz-time-tooltip__note giz-time-tooltip__note--warn" : "giz-time-tooltip__note";

        protected string ExpireNoteIcon => SoleProductInCredit ? "ph-warning-circle" : "ph-hourglass-medium";

        protected string TopUpButtonClass => CanShop ? "giz-time-tooltip__btn" : "giz-time-tooltip__btn giz-time-tooltip__btn--primary";

        protected static string RowClass(TimeProductViewState product)
        {
            var css = "giz-time-tooltip__row";

            if (product.ActivationOrder == 1)
                css += " giz-time-tooltip__row--current";

            if (product.InCredit)
                css += " giz-time-tooltip__row--credit";

            return css;
        }

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
