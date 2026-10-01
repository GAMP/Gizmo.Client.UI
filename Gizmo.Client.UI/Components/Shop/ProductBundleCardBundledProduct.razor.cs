using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductBundleCardBundledProduct : CustomDOMComponentBase
    {
        private UserProductViewState _product;

        [Inject]
        public UserProductViewState Product
        {
            get { return _product; }
            private set { _product = value; }
        }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public decimal Quantity { get; set; }

        private string IconClass => _product?.ProductType == Gizmo.Web.Api.Models.ProductType.ProductTime
            ? "ph-bold ph-clock"
            : "ph-bold ph-package";

        private string QuantityText => "×" + Quantity.ToString("N0", System.Globalization.CultureInfo.CurrentCulture);

        protected override async Task OnInitializedAsync()
        {
            _product = await UserProductViewStateLookupService.GetStateAsync(ProductId);

            if (_product != null)
            {
                this.SubscribeChange(_product);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_product != null)
            {
                this.UnsubscribeChange(_product);
            }

            base.Dispose();
        }
    }
}
