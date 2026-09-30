using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductTimeCardHover : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        private bool HasDescription => !string.IsNullOrWhiteSpace(Product?.Description);

        private string BuyWindow => Product?.PurchaseAvailability is null
            ? null
            : ProductHelpers.GetPurchaseAvailabilities(Product, true, LocalizationService).FirstOrDefault();

        private string PlayWindow => Product?.TimeProduct?.UsageAvailability is null
            ? null
            : ProductHelpers.GetUsageAvailabilities(Product, true, LocalizationService).FirstOrDefault();

        private string ExpiryText
        {
            get
            {
                var time = Product?.TimeProduct;

                if (time is null || time.ExpirationOptions == ProductTimeExpirationOptionType.None)
                    return null;

                var parts = new List<string>();

                if (time.ExpirationOptions.HasFlag(ProductTimeExpirationOptionType.ExpireAtDayTime))
                    parts.Add(ProductHelpers.GetDayTimeFromMinute(time.ExpireAtDayTimeMinute));

                if (time.ExpirationOptions.HasFlag(ProductTimeExpirationOptionType.ExpireAfterTime))
                    parts.Add(ProductHelpers.GetExpiresAfterText(time, LocalizationService).Trim());

                if (time.ExpirationOptions.HasFlag(ProductTimeExpirationOptionType.ExpiresAtLogout))
                    parts.Add(LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_AFTER_LOGOUT)));

                return string.Join(" · ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
            }
        }

        private string GetTimeText()
        {
            if (Product != null && Product.ProductType == ProductType.ProductTime)
            {
                return $"{Product.TimeProduct?.Minutes.ToString("N0")} {LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_MINUTES_ABBREVIATED))}";
            }

            return string.Empty;
        }

        protected override async Task OnInitializedAsync()
        {
            if (Product != null)
            {
                this.SubscribeChange(Product);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (Product != null)
            {
                this.UnsubscribeChange(Product);
            }

            base.Dispose();
        }
    }
}
