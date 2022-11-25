using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_HOME)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_HOME_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_HOME_TITLE"), ModuleDisplayOrder(0)]
    [Route(ClientRoutes.HomeRoute)]
    public partial class Home : CustomDOMComponentBase, IAsyncDisposable
    {
        public Home()
        {
        }

        #region FIELDS
        private int _selectedSearchCategoryId;
        #endregion

        #region PROPERTIES

        [Inject]
        HomePageService HomePageService { get; set; }

        [Inject]
        ExecutableSelectorService ExecutableSelectorService { get; set; }

        [Inject]
        AdvertisementsService AdvertisementsService { get; set; }

        [Inject]
        SearchService SearchService { get; set; }

        [Inject]
        ApplicationsPageService ApplicationsPageService { get; set; }

        [Inject]
        ShopPageService ShopPageService { get; set; }

        [Inject()]
        IClientDialogService DialogService { get; set; }

        public List<ApplicationSortOptionViewModel> SearchCategories { get; set; }

        #endregion

        #region METHODS

        public async Task OpenExecutableSelector(int id)
        {
            await ExecutableSelectorService.LoadApplicationAsync(id);

            var s = await DialogService.ShowExecutableSelectorDialogAsync();
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        public IEnumerable<ApplicationViewState> GetFilteredApplications()
        {
            var result = ApplicationsPageService.ViewState.Applications.AsQueryable();
            if (SearchService.ViewState.ShowAll)
            {
                var ids = SearchService.ViewState.ApplicationResults.Select(a => a.Id).ToList();

                result = result.Where(a => ids.Contains(a.Id));
            }

            return result.ToList();
        }

        public IEnumerable<ProductViewState> GetFilteredProducts()
        {
            var result = ShopPageService.ViewState.Products.AsQueryable();
            if (SearchService.ViewState.ShowAll)
            {
                var ids = SearchService.ViewState.ProductResults.Select(a => a.Id).ToList();

                result = result.Where(a => ids.Contains(a.Id));
            }

            return result.ToList();
        }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InvokeVoidAsync("registerHomeAdsAutoCollapse");
            }
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(HomePageService.ViewState);
            this.SubscribeChange(SearchService.ViewState);

            await HomePageService.LoadPopularProductsAsync();

            SearchCategories = new List<ApplicationSortOptionViewModel>();
            SearchCategories.Add(new ApplicationSortOptionViewModel() { Id = 0, Name = "Games & apps + shop" });
            SearchCategories.Add(new ApplicationSortOptionViewModel() { Id = 1, Name = "Games & apps" });
            SearchCategories.Add(new ApplicationSortOptionViewModel() { Id = 2, Name = "Shop" });

          

             await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(SearchService.ViewState);
            this.UnsubscribeChange(HomePageService.ViewState);

            base.Dispose();
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterHomeAdsAutoCollapse", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}