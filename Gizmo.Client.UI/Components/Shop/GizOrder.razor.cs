using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrder : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartViewService Service { get; set; }

        [Inject]
        ClientServerCartViewService ClientServerCartViewService { get; set; }

        public string GetPromocodeStatusClass(Web.Api.Models.PromoCodeApplyStatus status)
        {
            string result = "giz-order-promocode-status";

            switch (status)
            {
                case Web.Api.Models.PromoCodeApplyStatus.Applied:
                    result += " giz-order-promocode-status--applied";
                    break;

                case Web.Api.Models.PromoCodeApplyStatus.NotApplied:
                    result += " giz-order-promocode-status--not-applied";
                    break;

                case Web.Api.Models.PromoCodeApplyStatus.Unusable:
                    result += " giz-order-promocode-status--unusable";
                    break;
            }

            return result;
        }

        public string GetPromocodeStatusText(Web.Api.Models.PromoCodeApplyStatus status)
        {
            switch (status)
            {
                case Web.Api.Models.PromoCodeApplyStatus.Applied:
                    return LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_PROMOCODE_APPLIED));

                case Web.Api.Models.PromoCodeApplyStatus.NotApplied:
                    return LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_PROMOCODE_NOT_APPLIED));

                case Web.Api.Models.PromoCodeApplyStatus.Unusable:
                    return LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_PROMOCODE_UNUSABLE));
            }

            return string.Empty;
        }

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
