using System.Globalization;
using System.Linq;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrder : CustomDOMComponentBase
    {
        private bool _noteOpen;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartViewService Service { get; set; }

        [Inject]
        ClientServerCartViewService ClientServerCartViewService { get; set; }

        private int ProductCount => ClientServerCartViewService.ViewState.Products.Count();

        private bool HasProducts => ProductCount > 0;

        private bool IsNoteOpen => _noteOpen || !string.IsNullOrEmpty(Service.ViewState.Notes);

        private bool IsCartBusy => ClientServerCartViewService.ViewState.IsStateUpdateRequired
            || ClientServerCartViewService.ViewState.IsStateUpdating;

        private bool IsPromoBusy => IsCartBusy || ClientServerCartViewService.ViewState.PromoCodeViewState.IsLoading;

        private bool HasPromoCode => !string.IsNullOrEmpty(ClientServerCartViewService.ViewState.PromoCodeViewState.PromoCode);

        private bool IsPromoLocked => ClientServerCartViewService.ViewState.PromoCodeStatus != PromoCodeApplyStatus.None;

        private bool IsApplyDisabled => IsPromoBusy
            || string.IsNullOrWhiteSpace(ClientServerCartViewService.ViewState.PromoCodeViewState.InputPromoCode);

        private bool IsOrderDisabled => IsCartBusy || !HasProducts;

        private string PromoClass => ClientServerCartViewService.ViewState.PromoCodeStatus switch
        {
            PromoCodeApplyStatus.Applied => "giz-cart__promo giz-cart__promo--applied",
            PromoCodeApplyStatus.NotApplied or PromoCodeApplyStatus.Unusable => "giz-cart__promo giz-cart__promo--refused",
            _ => "giz-cart__promo",
        };

        private string PromoStatusClass => ClientServerCartViewService.ViewState.PromoCodeStatus == PromoCodeApplyStatus.Applied
            ? "giz-cart__promo-status giz-cart__promo-status--applied"
            : "giz-cart__promo-status";

        private string PromoStatusText => ClientServerCartViewService.ViewState.PromoCodeStatus switch
        {
            PromoCodeApplyStatus.Applied => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_PROMOCODE_APPLIED)),
            PromoCodeApplyStatus.NotApplied => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_PROMOCODE_NOT_APPLIED)),
            PromoCodeApplyStatus.Unusable => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_PROMOCODE_UNUSABLE)),
            _ => ClientServerCartViewService.ViewState.PromotionExceptionMessage,
        };

        private string TotalText => ClientServerCartViewService.ViewState.Total.ToString("C", CultureInfo.CurrentCulture);

        private string PointsTotalText => ClientServerCartViewService.ViewState.PointsTotal.ToString("N0", CultureInfo.CurrentCulture);

        private string AwardText => "+" + ClientServerCartViewService.ViewState.PointsAward.ToString("N0", CultureInfo.CurrentCulture);

        private void OpenNote() => _noteOpen = true;

        protected override void OnInitialized()
        {
            this.SubscribeChange(Service.ViewState);
            this.SubscribeChange(ClientServerCartViewService.ViewState);
            this.SubscribeChange(ClientServerCartViewService.ViewState.PromoCodeViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ClientServerCartViewService.ViewState.PromoCodeViewState);
            this.UnsubscribeChange(ClientServerCartViewService.ViewState);
            this.UnsubscribeChange(Service.ViewState);

            base.Dispose();
        }
    }
}
