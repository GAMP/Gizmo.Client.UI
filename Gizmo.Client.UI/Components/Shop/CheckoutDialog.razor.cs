using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class CheckoutDialog : CartDialogBase
    {
        #region FIELDS

        private UserProductViewState _package;
        private int? _packageProductId;
        private string _doneName;
        private string _doneDuration;

        #endregion

        #region PROPERTIES

        [Inject] UserProductViewStateLookupService ProductLookupService { get; set; }

        #endregion

        #region THE CART

        protected override bool IsPriced => CartService.ViewState.Products.Any();

        private IEnumerable<UserCartProductViewState> Items =>
            CartService.ViewState.Products.OrderBy(a => a.Number);

        private int ItemCount => CartService.ViewState.Products.Sum(a => a.Quantity);

        private string ItemsTitle => GrafitLocalization.GetPluralString(GrafitResourceKeys.SHELL_BUY_ITEMS_COUNT, ItemCount);

        private static bool IsPointsLine(UserCartProductViewState item) =>
            item.PayType == OrderLinePayType.Points;

        #endregion

        #region A SINGLE TIME PACKAGE

        private UserCartProductViewState PackageLine
        {
            get
            {
                var products = CartService.ViewState.Products;

                return products.Count() == 1 && products.First() is { ProductEntityType: ProductType.ProductTime, Quantity: 1 } line
                    ? line
                    : null;
            }
        }

        private bool IsPackage => PackageLine is not null;

        private string Title => IsPackage ? PackageLine.ProductName : ItemsTitle;

        private string Duration
        {
            get
            {
                var minutes = _package?.TimeProduct?.Minutes ?? 0;

                if (minutes <= 0)
                    return string.Empty;

                if (minutes < 60)
                    return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DURATION_MINUTES, minutes);

                var hours = minutes / 60;
                var rest = minutes % 60;

                return rest == 0
                    ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DURATION_HOURS, hours)
                    : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DURATION_HOURS_MINUTES, hours, rest);
            }
        }

        private string CataloguePrice
        {
            get
            {
                var line = PackageLine;

                if (line is null)
                    return string.Empty;

                var money = line.UnitPrice;
                var points = line.UnitPointsPrice ?? 0;

                if (money <= 0 && points <= 0)
                    return GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_PRICE_FREE));

                if (money > 0 && points > 0)
                {
                    var joiner = GrafitLocalization.GetString(line.PurchaseOptions == PurchaseOptionType.Or
                        ? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_PRICE_PURCHASE_OPTION_OR)
                        : nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_PRICE_PURCHASE_OPTION_AND));

                    return $"{Money(money)} {joiner} {PointsWithUnit(points)}";
                }

                return money > 0 ? Money(money) : PointsWithUnit(points);
            }
        }

        private bool ShowLines => HasDiscount || Fees > 0;

        #endregion

        #region POINTS FOR THE WHOLE ORDER

        private static bool IsEitherWay(UserCartProductViewState line) =>
            line.PurchaseOptions == PurchaseOptionType.Or && line.UnitPointsPrice > 0 && line.UnitPrice > 0;

        private IEnumerable<UserCartProductViewState> EitherWayLines => CartService.ViewState.Products.Where(IsEitherWay);

        private static int LinePoints(UserCartProductViewState line) =>
            line.PayType == OrderLinePayType.Points && line.TotalPointsPrice > 0
                ? line.TotalPointsPrice.Value
                : line.UnitPointsPrice.GetValueOrDefault() * line.Quantity;

        private int PointsAlwaysDue => CartService.ViewState.Products
            .Where(a => !IsEitherWay(a))
            .Sum(a => a.TotalPointsPrice.GetValueOrDefault());

        private int PointsToSpend => PointsBalance - PointsAlwaysDue;

        protected override bool OffersPoints => _pointsAllowed && EitherWayLines.Any();

        protected override bool IsPayingWithPoints => EitherWayLines.Any(a => a.PayType == OrderLinePayType.Points);

        protected override bool WayIsShort(PayWay way) => way.Kind == PayWayKind.Points
            ? IsPriced && !EitherWayLines.Any(a => LinePoints(a) <= PointsToSpend)
            : base.WayIsShort(way);

        private IEnumerable<PayWay> MoneyWays => _ways.Where(a => a.Kind != PayWayKind.Points);

        private PayWay MoneyWay
        {
            get
            {
                var methodId = CartOrderService.ViewState.PaymentMethodId;

                return methodId is null ? null : MoneyWays.FirstOrDefault(a => a.MethodId == methodId);
            }
        }

        private bool ShowRest => IsPayingWithPoints && IsPriced && Total > 0 && MoneyWays.Any();

        private string RestCaption => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_REST_WITH, Money(Total));

        protected override void Select(PayWay way)
        {
            if (way is null || IsSelected(way) || _paying)
                return;

            if (way.Kind == PayWayKind.Points)
            {
                SelectPoints();
                StateHasChanged();
                return;
            }

            foreach (var line in EitherWayLines.Where(a => a.PayType == OrderLinePayType.Points))
                CartService.SetPayType(line.Guid, OrderLinePayType.Cash);

            base.Select(way);
        }

        protected override void SelectPoints()
        {
            var left = PointsToSpend;

            foreach (var line in EitherWayLines.OrderByDescending(LinePoints).ThenBy(a => a.Number))
            {
                var points = LinePoints(line);
                var wanted = points <= left ? OrderLinePayType.Points : OrderLinePayType.Cash;

                if (wanted == OrderLinePayType.Points)
                    left -= points;

                if (line.PayType != wanted)
                    CartService.SetPayType(line.Guid, wanted);
            }

            if (MoneyWay is not null)
                return;

            var rest = MoneyWays.FirstOrDefault(a => a.Kind == PayWayKind.Balance) ?? MoneyWays.FirstOrDefault();

            if (rest is not null)
                CartOrderService.SetOrderPaymentMethod(rest.MethodId);
        }

        private void SelectRest(PayWay way)
        {
            if (way is null || _paying || way == MoneyWay)
                return;

            CartOrderService.SetOrderPaymentMethod(way.MethodId);
            StateHasChanged();
        }

        #endregion

        #region PAYING

        protected override string TitleKey => IsPackage
            ? GrafitResourceKeys.SHELL_BUY_TITLE
            : nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_CHECKOUT_TITLE);

        protected override string PayForKey => IsPackage
            ? GrafitResourceKeys.SHELL_BUY_BUY_FOR
            : GrafitResourceKeys.SHELL_BUY_ORDER_FOR;

        private string DoneTitle => _paidWith == PayWayKind.Counter
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_ORDERED_TITLE)
            : GrafitLocalization.GetString(_doneName is null ? GrafitResourceKeys.SHELL_BUY_ORDER_PAID_TITLE : GrafitResourceKeys.SHELL_BUY_DONE_TITLE);

        private string DoneNote => _paidWith == PayWayKind.Counter
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_ORDERED_HINT)
            : _doneDuration ?? string.Empty;

        protected override void Pay()
        {
            if (!CanPay)
                return;

            _paying = true;
            _paidWith = SelectedWay?.Kind ?? PayWayKind.Balance;
            _doneName = PackageLine?.ProductName;
            _doneDuration = IsPackage ? Duration : null;
            StateHasChanged();

            DispatchWorkflow(async () =>
            {
                try
                {
                    if (Total == 0 && CartOrderService.ViewState.PaymentMethodId.HasValue)
                    {
                        CartOrderService.SetOrderPaymentMethod(null);
                        await WaitForCartAsync();
                    }

                    await CartOrderService.CheckoutAsync();

                    var state = CartOrderService.ViewState;

                    if (state.IsComplete)
                    {
                        _outcomeOk = !state.HasError;
                        _outcomeError = state.ErrorMessage;
                        _step = Step.Done;
                    }
                }
                finally
                {
                    _paying = false;

                    if (!IsDisposed)
                        StateHasChanged();
                }
            });
        }

        protected override async Task CloseDialog()
        {
            if (_paying)
                return;

            LeaveTopUp();

            await DismissCallback.InvokeAsync();

            CartOrderService.ClearDialog();
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var productId = PackageLine?.ProductId;

            if (productId is { } id && _packageProductId != id)
            {
                _packageProductId = id;

                try
                {
                    _package = await ProductLookupService.GetStateAsync(id);
                }
                catch (Exception)
                {
                    _package = null;
                }

                if (!IsDisposed)
                    StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}
