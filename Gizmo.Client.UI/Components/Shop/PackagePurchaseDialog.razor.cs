using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Server.Exceptions;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    /// <summary>
    /// Buying a time package in one step, without a trip through the shop cart.
    /// </summary>
    /// <remarks>
    /// <para>
    /// "Bypassing the cart" means bypassing the cart SCREEN, not the cart itself - see
    /// <see cref="CartDialogBase"/>. The caller puts the package in the cart before
    /// opening this dialog, so every figure shown is the real, server computed one, and
    /// the package is taken back out again if the customer closes without paying.
    /// Nothing is left behind either way.
    /// </para>
    /// <para>
    /// Points are offered here, on the line, because they used to be unreachable: the
    /// choice lived on the cart screen, and closing this dialog to get there removed the
    /// package.
    /// </para>
    /// <para>
    /// The order is accepted through <see cref="ClientServerCartViewService"/> directly
    /// rather than through the shop's <c>UserCartViewService.CheckoutAsync</c>. That method
    /// validates the balance against a cached figure before talking to the server and
    /// returns silently when the check fails, which cannot be told apart from success from
    /// the outside; a stale cache after a lost balance event then reported a purchase that
    /// never happened. The server is the only authority on the balance.
    /// </para>
    /// </remarks>
    public partial class PackagePurchaseDialog : CartDialogBase
    {
        #region FIELDS

        private UserProductViewState _product;
        private bool _paid;

        #endregion

        #region PROPERTIES

        [Inject] IAssemblyResourcesLocalizationService AssemblyLocalizationService { get; set; }
        [Inject] UserProductViewStateLookupService ProductLookupService { get; set; }

        [Parameter] public int ProductId { get; set; }

        /// <summary>
        /// The cart entry this dialog shows and pays for.
        /// </summary>
        /// <remarks>
        /// The caller puts the package in the cart before opening the dialog and passes the
        /// finished entry in. Adding it here used to hang the dialog on a package with a
        /// usage availability window: the check in ClientServerCartViewService raises a
        /// "not available right now, add anyway?" prompt and waits, and dialogs are a
        /// queue, so that prompt sat behind this one and never reached the screen.
        /// </remarks>
        [Parameter] public Guid CartEntryId { get; set; }

        #endregion

        #region THE PACKAGE

        private UserCartProductViewState Entry =>
            CartService.ViewState.Products.FirstOrDefault(a => a.Guid == CartEntryId);

        protected override bool IsPriced => Entry is not null;

        private string ProductName => _product?.Name ?? ShellStringOverrides.Get(ShellStringOverrides.BUY_FALLBACK_NAME);

        private string Duration
        {
            get
            {
                var minutes = _product?.TimeProduct?.Minutes ?? 0;

                if (minutes <= 0)
                    return string.Empty;

                if (minutes < 60)
                    return ShellStringOverrides.Get(ShellStringOverrides.DURATION_MINUTES, minutes);

                var hours = minutes / 60;
                var rest = minutes % 60;

                return rest == 0
                    ? ShellStringOverrides.Get(ShellStringOverrides.DURATION_HOURS, hours)
                    : ShellStringOverrides.Get(ShellStringOverrides.DURATION_HOURS_MINUTES, hours, rest);
            }
        }

        /// <summary>
        /// The price as the catalogue states it, under the name: money, points, "money or
        /// points", "money and points", or free. What the cart then charges is the receipt's
        /// business; this line only says what the package costs.
        /// </summary>
        private string CataloguePrice
        {
            get
            {
                if (_product is null)
                    return string.Empty;

                var money = _product.UnitPrice;
                var points = _product.UnitPointsPrice ?? 0;

                if (money <= 0 && points <= 0)
                    return ShellStringOverrides.Get(ShellStringOverrides.PRICE_FREE);

                if (money > 0 && points > 0)
                {
                    var joiner = ShellStringOverrides.Get(_product.PurchaseOptions == PurchaseOptionType.Or
                        ? ShellStringOverrides.PRICE_OR
                        : ShellStringOverrides.PRICE_AND);

                    return $"{Money(money)} {joiner} {PointsWithUnit(points)}";
                }

                return money > 0 ? Money(money) : PointsWithUnit(points);
            }
        }

        /// <summary>The package line as the server priced it, in money.</summary>
        private decimal PackageTotal => Entry?.TotalPrice ?? 0;

        /// <summary>
        /// What the other things in the cart cost, when there are any.
        /// </summary>
        /// <remarks>
        /// There is one cart per user, so paying now pays for those items too. Saying so,
        /// with the amount, is the only honest option - quietly charging for a basket the
        /// customer thought they had left for later would be worse than the extra line.
        /// </remarks>
        private decimal OtherItemsTotal =>
            CartService.ViewState.Products
                .Where(a => a.Guid != CartEntryId && a.PayType != OrderLinePayType.Points)
                .Sum(a => a.TotalPrice);

        private bool HasOtherItems => CartService.ViewState.Products.Any(a => a.Guid != CartEntryId);

        /// <summary>
        /// The receipt's lines above the total earn their place only when they add up to
        /// something other than the package's own price: "price 650, to pay 650" is the
        /// same number twice.
        /// </summary>
        private bool ShowLines => HasOtherItems || HasDiscount || Fees > 0;

        #endregion

        #region POINTS

        /// <summary>
        /// Points buy the package outright only when the product says "money OR points";
        /// "money AND points" is a surcharge the server adds by itself.
        /// </summary>
        protected override bool OffersPoints =>
            _product is not null && _product.PurchaseOptions == PurchaseOptionType.Or && (_product.UnitPointsPrice ?? 0) > 0;

        protected override bool IsPayingWithPoints => Entry?.PayType == OrderLinePayType.Points;

        /// <summary>
        /// Points the server will take for the whole cart.
        /// </summary>
        /// <remarks>
        /// Right after switching to points the cart has not been re-priced yet and still
        /// reports zero; the package's own points price stands in until it has, so the
        /// figure does not flash from zero to the real one.
        /// </remarks>
        protected override int PointsDue
        {
            get
            {
                var fromCart = CartService.ViewState.PointsTotal;

                if (fromCart > 0)
                    return fromCart;

                return IsPayingWithPoints ? (_product?.UnitPointsPrice ?? 0) : 0;
            }
        }

        /// <summary>
        /// The package's money price as a cell judges it: the cart's figure, or the
        /// catalogue price while the cart is switched to points and reports the package
        /// at zero.
        /// </summary>
        private decimal MoneyPrice => IsPayingWithPoints ? (_product?.UnitPrice ?? 0) : PackageTotal;

        /// <summary>The package's points price as a cell judges it, by the same rule.</summary>
        private int PointsPrice => IsPayingWithPoints ? PointsDue : (_product?.UnitPointsPrice ?? 0);

        protected override bool WayIsShort(PayWay way) => way.Kind switch
        {
            PayWayKind.Balance => IsPriced && MoneyPrice > Balance,
            PayWayKind.Points => IsPriced && PointsPrice > PointsBalance,
            _ => false,
        };

        /// <summary>
        /// Pays for the package with <paramref name="way"/>.
        /// </summary>
        /// <remarks>
        /// Points are a pay type on the cart LINE; money methods are a payment method on
        /// the ORDER. Choosing points switches the line and leaves the order method alone:
        /// anything else in the cart is still paid in money, and a method bound to a cart
        /// that ends up at zero is dropped at the moment of paying. Choosing money puts the
        /// line back to cash and binds the method. Both requests are optimistic in the cart
        /// service, so the cells switch at once and the figures follow when the server has
        /// re-priced the cart.
        /// </remarks>
        protected override void Select(PayWay way)
        {
            if (way is null || IsSelected(way) || _paying || Entry is null)
                return;

            if (way.Kind == PayWayKind.Points)
            {
                SelectPoints();
            }
            else
            {
                if (IsPayingWithPoints)
                    CartService.SetPayType(CartEntryId, OrderLinePayType.Cash);

                base.Select(way);
            }

            StateHasChanged();
        }

        protected override void SelectPoints() => CartService.SetPayType(CartEntryId, OrderLinePayType.Points);

        #endregion

        #region PAYING

        protected override string TitleKey => ShellStringOverrides.BUY_TITLE;

        protected override string PayForKey => ShellStringOverrides.BUY_BUY_FOR;

        private string DoneTitle => _paidWith == PayWayKind.Counter
            ? ShellStringOverrides.Get(ShellStringOverrides.BUY_ORDERED_TITLE)
            : ShellStringOverrides.Get(ShellStringOverrides.BUY_DONE_TITLE);

        private string DoneNote => _paidWith == PayWayKind.Counter
            ? ShellStringOverrides.Get(ShellStringOverrides.BUY_ORDERED_HINT)
            : Duration;

        /// <summary>
        /// Places the order.
        /// </summary>
        /// <remarks>
        /// The error handling mirrors the shop's checkout: cart and promotion error codes
        /// are translated to the vendor's own messages, anything else is the generic line.
        /// After an accepted order the cart is reset, as the shop does - the accepted cart
        /// no longer exists on the server. A refused order leaves the cart as it was, so
        /// the customer's other goods survive and the package is taken back on close.
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
                try
                {
                    //A cart at zero money must not carry a payment method - the server
                    //prices a points line at zero and refuses a method bound to nothing.
                    //The stock checkout does the same by hiding the selector at zero.
                    if (Total == 0 && CartOrderService.ViewState.PaymentMethodId.HasValue)
                    {
                        CartOrderService.SetOrderPaymentMethod(null);
                        await WaitForCartAsync();
                    }

                    await CartService.AcceptAsync(null);

                    _paid = true;
                    _outcomeOk = true;
                }
                catch (WebApiClientException exception) when (exception.ErrorCode.HasValue && exception.IsExceptionCode(ExceptionCode.Cart))
                {
                    _outcomeOk = false;
                    _outcomeError = AssemblyLocalizationService.GetLocalizedStringValue((CartErrorCode)exception.ErrorCode.Value);
                }
                catch (WebApiClientException exception) when (exception.ErrorCode.HasValue && exception.IsExceptionCode(ExceptionCode.Promotion))
                {
                    _outcomeOk = false;
                    _outcomeError = AssemblyLocalizationService.GetLocalizedStringValue((PromotionErrorCode)exception.ErrorCode.Value);
                }
                catch (Exception exception)
                {
                    Logger?.LogError(exception, "Package purchase failed.");

                    _outcomeOk = false;
                    _outcomeError = ShellStringOverrides.Get(ShellStringOverrides.GEN_ERROR);
                }

                if (_paid)
                {
                    //The accepted cart is gone from the server; the local one follows. A
                    //failure here is reported by the cart service itself and does not
                    //change the fact that the package was bought.
                    await CartService.ResetAsync();
                }

                _paying = false;
                _step = Step.Done;
                StateHasChanged();
            });
        }

        protected override async Task CloseDialog()
        {
            if (_paying)
                return;

            TakeBackPackage();

            await DismissCallback.InvokeAsync();
        }

        /// <summary>
        /// Removes exactly the entry this dialog was opened for. Anything the customer put
        /// in the cart themselves stays.
        /// </summary>
        private void TakeBackPackage()
        {
            if (_paid || _paying || CartEntryId == Guid.Empty)
                return;

            if (CartService.ViewState.Products.Any(a => a.Guid == CartEntryId))
                CartService.RemoveEntry(CartEntryId);
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnLoadingAsync()
        {
            try
            {
                _product = await ProductLookupService.GetStateAsync(ProductId);
            }
            catch (Exception)
            {
                _product = null;
            }
        }

        public override void Dispose()
        {
            //Safety net for a teardown that did not go through CloseDialog.
            TakeBackPackage();

            base.Dispose();
        }

        #endregion
    }
}
