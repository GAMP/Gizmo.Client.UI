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
        private UserProductGroupViewState _userProductGroupViewState;
        private int _previousProductId;
        private bool _showMore = false;
        #endregion

        #region PROPERTIES

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserProductGroupViewStateLookupService UserProductGroupViewStateLookupService { get; set; }

        [Inject()]
        UserProductGroupViewState UserProductGroupViewState
        {
            get { return _userProductGroupViewState; }
            set { _userProductGroupViewState = value; }
        }
        [Inject]
        ProductDetailsPageViewState ViewState { get; set; }

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

        protected override async Task OnParametersSetAsync()
        {
            if (_previousProductId != ProductId)
            {
                if (_userProductGroupViewState != null)
                    this.UnsubscribeChange(_userProductGroupViewState);

                _previousProductId = ProductId;

                _userProductGroupViewState = await UserProductGroupViewStateLookupService.GetStateAsync(ViewState.Product.ProductGroupId); //TODO: A CHECK

                if (_userProductGroupViewState != null)
                    this.SubscribeChange(_userProductGroupViewState);
            }

            await base.OnParametersSetAsync();
        }

        public override void Dispose()
        {
            if (_userProductGroupViewState != null)
                this.UnsubscribeChange(_userProductGroupViewState);

            base.Dispose();
        }

        #endregion
    }
}
