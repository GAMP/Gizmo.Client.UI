using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
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

        private string TimeNumber => ProductHelpers.GetProductTimeNumber(Product);

        private string TimeUnit => ProductHelpers.GetProductTimeText(Product, GrafitLocalization);

        private AvailabilityShape.Day PlayDay => AvailabilityShape.Today(Product?.TimeProduct?.UsageAvailability, DateTime.Now);

        private static bool PlaysToday(AvailabilityShape.Day day) => day.Segments.Count > 0;

        private string TodayOffText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PEEK_TODAY_OFF);

        private IReadOnlyList<AvailabilityShape.WeekDay> BuyWeek =>
            AvailabilityShape.Week(Product?.PurchaseAvailability, CultureInfo.CurrentCulture);

        private static string DayClass(AvailabilityShape.WeekDay day) => day.On ? "giz-peek__weekday giz-peek__weekday--on" : "giz-peek__weekday";

        private string BuyWindow => Product?.PurchaseAvailability is null
            ? null
            : ProductHelpers.GetPurchaseAvailabilities(Product, true, LocalizationService).FirstOrDefault();

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
