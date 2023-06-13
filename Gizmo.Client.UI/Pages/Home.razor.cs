using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_HOME)]
    [PageUIModule(TitleLocalizationKey = "GIZ_MODULE_PAGE_HOME_TITLE", DescriptionLocalizationKey = "GIZ_MODULE_PAGE_HOME_TITLE"), ModuleDisplayOrder(0)]
    [Route(ClientRoutes.HomeRoute)]
    public partial class Home : CustomDOMComponentBase, IAsyncDisposable
    {
        #region PROPERTIES

        [Inject]
        IOptions<PopularItemsOptions> PopularItemsOptions { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        HomePageViewService HomePageService { get; set; }

        [Inject]
        HomePageViewState ViewState { get; set; }

        [Inject]
        AdvertisementsViewService AdvertisementsViewStateService { get; set; }

        [Inject]
        AdvertisementsViewState AdvertisementsViewState { get; set; }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InvokeVoidAsync("registerAdsAutoCollapse");
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
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
