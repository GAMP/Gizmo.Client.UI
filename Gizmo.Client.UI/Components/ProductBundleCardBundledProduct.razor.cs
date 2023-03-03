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

        private int _previousProductId;

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var productIdChanged = _previousProductId != ProductId;

            if (productIdChanged)
            {
                _previousProductId = ProductId;

                if (_product != null)
                {
                    //The same component used again with a different product id.
                    //We have to unbind from the old product.
                    this.UnsubscribeChange(_product);
                }

                //We have to bind to the new product.
                _product = await UserProductViewStateLookupService.GetStateAsync(ProductId);
                this.SubscribeChange(_product);
            }
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
