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
