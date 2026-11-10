using Gizmo.Client.UI.Localization.Resources;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductSimpleCardHover : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        private bool HasDescription => !string.IsNullOrWhiteSpace(Product?.Description);

        private bool HasDetails => HasDescription || BuyWeek.Count > 0 || AwardText.Length > 0;

        private string AwardText => Product?.UnitPointsAward is int award && award > 0
            ? "+" + GrafitLocalization.GetPluralString(GrafitResourceKeys.SHELL_BUY_POINTS_COUNT, award)
            : string.Empty;

        private IReadOnlyList<AvailabilityShape.WeekDay> BuyWeek =>
            AvailabilityShape.Week(Product?.PurchaseAvailability, CultureInfo.CurrentCulture);

        private static string DayClass(AvailabilityShape.WeekDay day) => day.On ? "giz-peek__weekday giz-peek__weekday--on" : "giz-peek__weekday";

        private string BuyWindow => Product?.PurchaseAvailability is null
            ? null
            : ProductHelpers.GetPurchaseAvailabilities(Product, true, LocalizationService).FirstOrDefault();

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
