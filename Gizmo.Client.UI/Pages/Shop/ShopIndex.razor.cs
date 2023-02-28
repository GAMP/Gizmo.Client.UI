using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_SHOP)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_SHOP_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_SHOP_DESCRIPTION"), ModuleDisplayOrder(2)]
    [Route(ClientRoutes.ShopRoute)]
    public partial class ShopIndex : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ShopPageService ShopService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Inject]
        SearchService SearchService { get; set; }

        #endregion



        #region METHODS

        private ProductViewState[] GetFilteredProducts()
        {
            var result = ShopService.ViewState.Products;

            if (SearchService.ViewState.ShowAll)
            {
                var ids = SearchService.ViewState.AppliedProductResults.Select(a => a.Id).ToList();

                result = result.Where(a => ids.Contains(a.Id));
            }

            if (ShopService.ViewState.SelectedUserProductGroupId.HasValue)
            {
                result = result.Where(a => a.ProductGroupId == ShopService.ViewState.SelectedUserProductGroupId.Value);
            }

            return result.ToArray();
        }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ShopService.ViewState);
            this.SubscribeChange(SearchService.ViewState);

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
