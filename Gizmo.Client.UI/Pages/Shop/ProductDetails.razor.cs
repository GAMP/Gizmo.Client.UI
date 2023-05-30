using System.Collections.Generic;
using System.Linq;
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
        #region FIELDS
        private UserCartProductItemViewState _productItemViewState;
        private UserProductGroupViewState _userProductGroupViewState;
        private int _previousProductId;
        private IEnumerable<UserHostGroupViewState> _hostGroups;
        private bool _showMore = false;
        private ElementReference _hostGroupContainer;
        #endregion

        #region PROPERTIES

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartProductItemViewStateLookupService UserCartProductItemViewStateLookupService { get; set; }

        [Inject]
        UserProductGroupViewStateLookupService UserProductGroupViewStateLookupService { get; set; }

        [Inject]
        ProductDetailsPageViewService ProductDetailsPageService { get; set; }

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

        [Inject]
        UserHostGroupViewStateLookupService UserHostGroupViewStateLookupService { get; set; }

        [Inject]
        HostGroupViewState HostGroupViewState { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int ProductId { get; set; }

        #endregion

        private Task OnClickBackButtonHandler()
        {
            return NavigationService.GoBackAsync();
        }

        private void ToggleMore()
        {
            _showMore = !_showMore;
        }

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            if (HostGroupViewState.HostGroupId.HasValue)
            {
                var hostGroups = await UserHostGroupViewStateLookupService.GetStatesAsync();
                var tmp = hostGroups.Where(a => a.Id != HostGroupViewState.HostGroupId.Value).ToList();
                var current = hostGroups.Where(a => a.Id == HostGroupViewState.HostGroupId.Value).FirstOrDefault();
                if (current != null)
                {
                    tmp.Insert(0, current);
                }
                _hostGroups = tmp;
            }
            else
            {
                _hostGroups = await UserHostGroupViewStateLookupService.GetStatesAsync();
            }

            await base.OnInitializedAsync();
        }

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
                this.SubscribeChange(_userProductGroupViewState);
            }

            await base.OnParametersSetAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await InvokeVoidAsync("productDetailsFitHostGroups", _hostGroupContainer);

            await base.OnAfterRenderAsync(firstRender);
        }

        public override void Dispose()
        {
            if (_userProductGroupViewState != null)
            {
                this.UnsubscribeChange(_userProductGroupViewState);
            }

            if (_productItemViewState != null)
            {
                this.UnsubscribeChange(_productItemViewState);
            }

            base.Dispose();
        }

        #endregion
    }
}
