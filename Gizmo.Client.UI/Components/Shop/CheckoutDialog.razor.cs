using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    /// <summary>
    /// Checkout of the shop cart: what is in it, what to pay with, what it comes to.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Opened by the shop's own <c>UserCartViewService.SubmitAsync</c> from the cart
    /// panel, and the order is placed through its <c>CheckoutAsync</c>: the shop owns
    /// this dialog's lifetime - it closes it when the cart is reset under it - and the
    /// flag that keeps it open through the reset is set inside that method.
    /// </para>
    /// <para>
    /// Points are not offered here: in the cart each line chooses money or points on the
    /// cart page, and the checkout only shows what those choices add up to.
    /// </para>
    /// </remarks>
    public partial class CheckoutDialog : CartDialogBase
    {
        #region THE CART

        protected override bool IsPriced => CartService.ViewState.Products.Any();

        private IEnumerable<UserCartProductViewState> Items =>
            CartService.ViewState.Products.OrderBy(a => a.Number);

        private int ItemCount => CartService.ViewState.Products.Sum(a => a.Quantity);

        private string ItemsTitle => ShellStringOverrides.GetPlural(ShellStringOverrides.BUY_ITEMS_COUNT, ItemCount);

        private static bool IsPointsLine(UserCartProductViewState item) =>
            item.PayType == OrderLinePayType.Points;

        #endregion

        #region PAYING

        protected override string TitleKey => ShellStringOverrides.SHOP_CHECKOUT_TITLE;

        protected override string PayForKey => ShellStringOverrides.BUY_ORDER_FOR;

        private string DoneTitle => _paidWith == PayWayKind.Counter
            ? ShellStringOverrides.Get(ShellStringOverrides.BUY_ORDERED_TITLE)
            : ShellStringOverrides.Get(ShellStringOverrides.BUY_ORDER_PAID_TITLE);

        private string DoneNote => _paidWith == PayWayKind.Counter
            ? ShellStringOverrides.Get(ShellStringOverrides.BUY_ORDERED_HINT)
            : string.Empty;

        /// <summary>
        /// Places the order through the shop's own checkout.
        /// </summary>
        /// <remarks>
        /// That method validates the balance against a cached figure before talking to
        /// the server and returns without a word when the check fails. It marks the order
        /// complete only once the server has answered, so "not complete" after it returns
        /// means the check failed: the balances are re-read and the dialog stays on the
        /// confirmation, where the shortfall now shows, rather than reporting success.
        /// </remarks>
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
