using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductBundleCardHover : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        private IReadOnlyList<AvailabilityShape.WeekDay> BuyWeek =>
            AvailabilityShape.Week(Product?.PurchaseAvailability, CultureInfo.CurrentCulture);

        private static string DayClass(AvailabilityShape.WeekDay day) => day.On ? "giz-peek__weekday giz-peek__weekday--on" : "giz-peek__weekday";

        private string BuyWindow => Product?.PurchaseAvailability is null
            ? null
            : ProductHelpers.GetPurchaseAvailabilities(Product, true, LocalizationService).FirstOrDefault();

        [Parameter]
        public UserProductViewState Product { get; set; }

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (Product != null)
            {
                this.SubscribeChange(Product);
            }

            base.OnInitialized();
        }

        public override void Dispose()
        {
            if (Product != null)
            {
                this.UnsubscribeChange(Product);
            }

            base.Dispose();
        }

        #endregion
    }
}
