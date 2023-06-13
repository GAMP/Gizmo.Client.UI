using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        #region PROPERTIES

        [Inject]
        IOptions<PopularItemsOptions> PopularItemsOptions { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ProductsPageViewService ShopService { get; set; }

        [Inject]
        ProductsPageViewState ViewState { get; set; }

        [Inject]
        AdvertisementsViewService AdvertisementsViewStateService { get; set; }

        [Inject]
        AdvertisementsViewState AdvertisementsViewState { get; set; }

        [Inject]
        UserProductGroupViewStateLookupService UserProductGroupViewStateLookupService { get; set; }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InvokeVoidAsync("registerAdsAutoCollapse");
            }
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ViewState);

            var productGroups = await UserProductGroupViewStateLookupService.GetStatesAsync();

            _userProductGroups = productGroups.ToDictionary(key => key.ProductGroupId, value => value);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterAdsAutoCollapse", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}
