using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class CheckoutDialog : CartDialogBase
    {
        #region THE CART

        protected override bool IsPriced => CartService.ViewState.Products.Any();

        private IEnumerable<UserCartProductViewState> Items =>
            CartService.ViewState.Products.OrderBy(a => a.Number);

        private int ItemCount => CartService.ViewState.Products.Sum(a => a.Quantity);

        private string ItemsTitle => GrafitLocalization.GetPluralString(GrafitResourceKeys.SHELL_BUY_ITEMS_COUNT, ItemCount);

        private static bool IsPointsLine(UserCartProductViewState item) =>
            item.PayType == OrderLinePayType.Points;

        #endregion

        #region PAYING

        protected override string TitleKey => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_CHECKOUT_TITLE);

        protected override string PayForKey => GrafitResourceKeys.SHELL_BUY_ORDER_FOR;

        private string DoneTitle => _paidWith == PayWayKind.Counter
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_ORDERED_TITLE)
            : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_ORDER_PAID_TITLE);

        private string DoneNote => _paidWith == PayWayKind.Counter
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_ORDERED_HINT)
            : string.Empty;

        protected override void Pay()
        {
            if (!CanPay)
                return;

            _paying = true;
            _paidWith = SelectedWay?.Kind ?? PayWayKind.Balance;
            StateHasChanged();

            DispatchWorkflow(async () =>
            {
                if (Total == 0 && CartOrderService.ViewState.PaymentMethodId.HasValue)
                {
                    CartOrderService.SetOrderPaymentMethod(null);
                    await WaitForCartAsync();
                }

                await CartOrderService.CheckoutAsync();

                var state = CartOrderService.ViewState;

                if (!state.IsComplete)
                {
                    await RefreshBalanceAsync();

                    _paying = false;
                    StateHasChanged();
                    return;
                }

                _outcomeOk = !state.HasError;
                _outcomeError = state.ErrorMessage;

                _paying = false;
                _step = Step.Done;
                StateHasChanged();
            });
        }

        protected override async Task CloseDialog()
        {
            if (_paying)
                return;

            await DismissCallback.InvokeAsync();

            CartOrderService.ClearDialog();
        }

        #endregion
    }
}
