using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.Options;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_SHOP)]
    [PageUIModule(TitleLocalizationKey = "GIZ_MODULE_PAGE_SHOP_TITLE", DescriptionLocalizationKey = "GIZ_MODULE_PAGE_SHOP_DESCRIPTION"), ModuleDisplayOrder(2)]
    [Route(ClientRoutes.ShopRoute)]
    public partial class ProductsIndex : CustomDOMComponentBase
    {
        private Dictionary<int, UserProductGroupViewState> _userProductGroups;
        private IReadOnlyDictionary<int, decimal> _ownPrices = CartPrices.None;

        #region PROPERTIES

        [Inject]
        IOptionsMonitor<ClientInterfaceOptions> ClientInterfaceOptions { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ProductsPageViewService ShopService { get; set; }

        [Inject]
        ProductsPageViewState ViewState { get; set; }

        [Inject]
        UserProductGroupViewStateLookupService UserProductGroupViewStateLookupService { get; set; }

        [Inject]
        ClientServerCartViewService CartService { get; set; }

        protected IReadOnlyDictionary<int, decimal> OwnPrices => _ownPrices;

        #endregion

        protected string GroupClass(int? group) => group == ViewState.SelectedUserProductGroupId
            ? "giz-shop-group giz-shop-group--active"
            : "giz-shop-group";

        protected string GroupPressed(int? group) => group == ViewState.SelectedUserProductGroupId ? "true" : "false";

        private void OnCartChanged(object sender, EventArgs e)
        {
            var prices = CartPrices.Of(CartService.ViewState.Products);
            if (CartPrices.Same(prices, _ownPrices))
                return;

            _ownPrices = prices;
            InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ViewState);

            _ownPrices = CartPrices.Of(CartService.ViewState.Products);
            CartService.ViewState.OnChange += OnCartChanged;

            var productGroups = await UserProductGroupViewStateLookupService.GetStatesAsync();

            _userProductGroups = productGroups.ToDictionary(key => key.ProductGroupId, value => value);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            CartService.ViewState.OnChange -= OnCartChanged;
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
