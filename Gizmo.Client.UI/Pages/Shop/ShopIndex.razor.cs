using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_SHOP)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_SHOP_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_SHOP_DESCRIPTION"), ModuleDisplayOrder(2)]
    [Route(ClientRoutes.ShopRoute)]
    public partial class ShopIndex : CustomDOMComponentBase
    {
        public ShopIndex()
        {
        }

        #region FIELDS
        private List<string> _sortOptions;
        #endregion

        #region PROPERTIES

        [Inject]
        ShopPageService ShopService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Inject]
        AdvertisementsService AdvertisementsService { get; set; }

        [Inject]
        SearchService SearchService { get; set; }

        public string SelectedSortOption { get; set; } = "Name";

        public List<string> SortOptions
        {
            get
            {
                if (_sortOptions == null)
                {
                    _sortOptions = new List<string>();
                    _sortOptions.Add("Name");
                    _sortOptions.Add("Price");
                }

                return _sortOptions;
            }
            set
            {
                _sortOptions = value;
            }
        }

        #endregion

        #region METHODS

        public IEnumerable<ProductViewState> GetFilteredProducts()
        {
            var result = ShopService.ViewState.Products.AsQueryable();

            if (SearchService.ViewState.ShowAllLocally || SearchService.ViewState.ShowAll)
            {
                var ids = SearchService.ViewState.ProductResults.Select(a => a.Id).ToList();

                result = result.Where(a => ids.Contains(a.Id));
            }

            if (ShopService.ViewState.SelectedProductGroupId.HasValue)
            {
                result = result.Where(a => a.ProductGroupId == ShopService.ViewState.SelectedProductGroupId.Value);
            }

            return result.ToList();
        }

        #endregion

        private Task SelectProductGroup(int? productGroupId)
        {
            return ShopService.SetSelectedProductGroup(productGroupId);
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ShopService.ViewState);
            this.SubscribeChange(SearchService.ViewState);

            await ShopService.LoadProductsAsync();

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(SearchService.ViewState);
            this.UnsubscribeChange(ShopService.ViewState);

            base.Dispose();
        }
    }
}