using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
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

        [Parameter] public Guid CartEntryId { get; set; }

        #endregion

        #region THE PACKAGE

        private UserCartProductViewState Entry =>
            CartService.ViewState.Products.FirstOrDefault(a => a.Guid == CartEntryId);

        protected override bool IsPriced => Entry is not null;

        private string ProductName => _product?.Name ?? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_FALLBACK_NAME);

        private string Duration
        {
            get
            {
                var minutes = _product?.TimeProduct?.Minutes ?? 0;

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
                if (_product is null)
                    return string.Empty;

                var money = _product.UnitPrice;
                var points = _product.UnitPointsPrice ?? 0;

                if (money <= 0 && points <= 0)
                    return GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_PRICE_FREE));

                if (money > 0 && points > 0)
                {
                    var joiner = GrafitLocalization.GetString(_product.PurchaseOptions == PurchaseOptionType.Or
                        ? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_PRICE_PURCHASE_OPTION_OR)
                        : nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_PRICE_PURCHASE_OPTION_AND));

                    return $"{Money(money)} {joiner} {PointsWithUnit(points)}";
                }

                return money > 0 ? Money(money) : PointsWithUnit(points);
            }
        }

        private decimal PackageTotal => Entry?.TotalPrice ?? 0;

        private decimal OtherItemsTotal =>
            CartService.ViewState.Products
                .Where(a => a.Guid != CartEntryId && a.PayType != OrderLinePayType.Points)
                .Sum(a => a.TotalPrice);

        private bool HasOtherItems => CartService.ViewState.Products.Any(a => a.Guid != CartEntryId);

        private bool ShowLines => HasOtherItems || HasDiscount || Fees > 0;

        #endregion

        #region POINTS

        protected override bool OffersPoints =>
            _product is not null && _product.PurchaseOptions == PurchaseOptionType.Or && (_product.UnitPointsPrice ?? 0) > 0;

        protected override bool IsPayingWithPoints => Entry?.PayType == OrderLinePayType.Points;

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

        private decimal MoneyPrice => IsPayingWithPoints ? (_product?.UnitPrice ?? 0) : PackageTotal;

        private int PointsPrice => IsPayingWithPoints ? PointsDue : (_product?.UnitPointsPrice ?? 0);

        protected override bool WayIsShort(PayWay way) => way.Kind switch
        {
            PayWayKind.Balance => IsPriced && MoneyPrice > Balance,
            PayWayKind.Points => IsPriced && PointsPrice > PointsBalance,
            _ => false,
        };

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

        protected override string TitleKey => GrafitResourceKeys.SHELL_BUY_TITLE;

        protected override string PayForKey => GrafitResourceKeys.SHELL_BUY_BUY_FOR;

        private string DoneTitle => _paidWith == PayWayKind.Counter
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_ORDERED_TITLE)
            : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_DONE_TITLE);

        private string DoneNote => _paidWith == PayWayKind.Counter
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_ORDERED_HINT)
            : Duration;

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
                    _outcomeError = GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                }

                if (_paid)
                {
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
            TakeBackPackage();

            base.Dispose();
        }

        #endregion
    }
}
