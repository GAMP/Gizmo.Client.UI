using System.Threading.Tasks;

using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.ProductDetailsRoute)]
    public partial class ProductDetails : CustomDOMComponentBase
    {
        public ProductDetails()
        {
        }

        private UserCartProductItemViewState _productItemViewState;

        private int _previousProductId;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartProductItemViewStateLookupService UserCartProductItemViewStateLookupService { get; set; }

        [Inject]
        ProductDetailsPageService ProductDetailsPageService { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int ProductId { get; set; }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var orderLineChanged = _previousProductId != ProductId;

            if (orderLineChanged)
            {
                if (_productItemViewState != null)
                {
                    //The same component used again with a different product.
                    //We have to unbind from the old product.
                    this.UnsubscribeChange(_productItemViewState);
                }

                _previousProductId = ProductId;

                _productItemViewState = await UserCartProductItemViewStateLookupService.GetStateAsync(ProductId);

                //We have to bind to the new product.
                this.SubscribeChange(_productItemViewState);
            }
        }

        public override void Dispose()
        {
            if (_productItemViewState != null)
            {
                this.UnsubscribeChange(_productItemViewState);
            }
            base.Dispose();
        }
    }
}
