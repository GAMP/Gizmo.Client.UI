using System.Globalization;
using System.Threading.Tasks;

using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrderItem : CustomDOMComponentBase
    {
        private UserProductViewState _product;

        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        public UserProductViewState Product
        {
            get { return _product; }
            private set { _product = value; }
        }

        [Inject]
        ClientServerCartViewService ClientServerCartViewService { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Parameter]
        public UserCartProductViewState UserCartProductViewState { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        [Parameter]
        public EventCallback OnToggle { get; set; }

        private string ClassName => IsOpen ? "giz-cart-line giz-cart-line--open" : "giz-cart-line";

        private bool IsTimeProduct => _product?.ProductType == ProductType.ProductTime;

        private string PlaceholderIcon => IsTimeProduct ? "ph-fill ph-clock" : "ph-fill ph-package";

        private string QuantityText => UserCartProductViewState.Quantity.ToString(CultureInfo.CurrentCulture);

        private bool IsSingle => UserCartProductViewState.Quantity <= 1;

        private string LessIcon => IsSingle ? "ph-bold ph-trash" : "ph-bold ph-minus";

        private string LessClass => IsSingle
            ? "giz-cart-line__step giz-cart-line__step--remove"
            : "giz-cart-line__step";

        private string LessTitle => IsSingle
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_CART_REMOVE)
            : null;

        private string UnitPriceText => UserCartProductViewState.Quantity > 1 && UserCartProductViewState.TotalPrice > 0
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_CART_EACH,
                (UserCartProductViewState.TotalPrice / UserCartProductViewState.Quantity).ToString("C", CultureInfo.CurrentCulture))
            : null;

        private bool IsAtMaximum => IsTimeProduct && UserCartProductViewState.Quantity >= 1;

        private bool HasPayChoice => _product?.PurchaseOptions.HasFlag(PurchaseOptionType.Or) == true;

        private bool HasPointsPrice => UserCartProductViewState.TotalPointsPrice.GetValueOrDefault() > 0;

        private bool PaysWithPoints => HasPayChoice && UserCartProductViewState.PayType == OrderLinePayType.Points;

        private bool ShowsPoints => HasPayChoice ? PaysWithPoints : HasPointsPrice;

        private bool ShowsMoney => HasPayChoice
            ? !PaysWithPoints
            : UserCartProductViewState.TotalPrice > 0 || !HasPointsPrice;

        private string PriceText => UserCartProductViewState.TotalPrice.ToString("C", CultureInfo.CurrentCulture);

        private string PointsPriceText => UserCartProductViewState.TotalPointsPrice.GetValueOrDefault().ToString("N0", CultureInfo.CurrentCulture);

        private string CashPriceText => (PaysWithPoints
            ? (_product?.UnitPrice ?? 0) * UserCartProductViewState.Quantity
            : UserCartProductViewState.TotalPrice).ToString("C", CultureInfo.CurrentCulture);

        private string PointsWayText => (PaysWithPoints
            ? UserCartProductViewState.TotalPointsPrice.GetValueOrDefault()
            : (_product?.UnitPointsPrice ?? 0) * UserCartProductViewState.Quantity).ToString("N0", CultureInfo.CurrentCulture);

        private string PayClass(OrderLinePayType payType) => UserCartProductViewState.PayType == payType
            ? "giz-cart-line__pay-way giz-cart-line__pay-way--on"
            : "giz-cart-line__pay-way";

        private Task Toggle() => OnToggle.InvokeAsync();

        private Task OnRowKey(KeyboardEventArgs args) =>
            args.Key is "Enter" or " " ? Toggle() : Task.CompletedTask;

        private void OnLess(MouseEventArgs args)
        {
            if (IsSingle)
                ClientServerCartViewService.RemoveEntry(UserCartProductViewState.Guid);
            else
                OnRemoveQuantityButtonClickHandler(args);
        }

        public void OnRemoveQuantityButtonClickHandler(MouseEventArgs _) =>
            ClientServerCartViewService.SetQuantity(UserCartProductViewState.Guid, UserCartProductViewState.Quantity - 1);

        public void OnAddQuantityButtonClickHandlerAsync(MouseEventArgs _) =>
            ClientServerCartViewService.SetQuantity(UserCartProductViewState.Guid, UserCartProductViewState.Quantity + 1);

        public void SetPayType(bool isChecked, Web.Api.Models.OrderLinePayType payType)
        {
            if (isChecked)
                ClientServerCartViewService.SetPayType(UserCartProductViewState.Guid, payType);
        }

        protected override async Task OnInitializedAsync()
        {
            if (UserCartProductViewState != null)
            {
                this.SubscribeChange(UserCartProductViewState);

                if (UserCartProductViewState.ProductId.HasValue)
                {
                    _product = await UserProductViewStateLookupService.GetStateAsync(UserCartProductViewState.ProductId.Value);

                    if (_product != null)
                        this.SubscribeChange(_product);
                }
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_product != null)
                this.UnsubscribeChange(_product);

            if (UserCartProductViewState != null)
                this.UnsubscribeChange(UserCartProductViewState);

            base.Dispose();
        }
    }
}
