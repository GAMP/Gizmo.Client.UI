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
