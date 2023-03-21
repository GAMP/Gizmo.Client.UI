using System.Collections.Generic;
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
        private UserProductGroupViewState _userProductGroupViewState;
        private int _previousProductId;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartProductItemViewStateLookupService UserCartProductItemViewStateLookupService { get; set; }

        [Inject]
        UserProductGroupViewStateLookupService UserProductGroupViewStateLookupService { get; set; }

        [Inject]
        ProductDetailsPageService ProductDetailsPageService { get; set; }

        [Inject()]
        UserCartProductItemViewState ProductItemViewState
        {
            get { return _productItemViewState; }
            set { _productItemViewState = value; }
        }

        [Inject()]
        UserProductGroupViewState ProductGroupViewState
        {
            get { return _userProductGroupViewState; }
            set { _userProductGroupViewState = value; }
        }

        [Inject]
        ProductDetailsPageViewState ViewState { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int ProductId { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var productChanged = _previousProductId != ProductId;

            if (productChanged)
            {
                if (_productItemViewState != null)
                {
                    //The same component used again with a different product.
                    //We have to unbind from the old product.
                    this.UnsubscribeChange(_productItemViewState);
                }

                _previousProductId = ProductId;

                _productItemViewState = await UserCartProductItemViewStateLookupService.GetStateAsync(ProductId);
                _userProductGroupViewState = await UserProductGroupViewStateLookupService.GetStateAsync(ViewState.Product.ProductGroupId); //TODO: A CHECK

                //We have to bind to the new product.
                this.SubscribeChange(_productItemViewState);
            }

            await base.OnParametersSetAsync();
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
