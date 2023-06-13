using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_APPS)]
    [ModuleDisplayOrder(1)]
    [PageUIModule(TitleLocalizationKey = "GIZ_MODULE_PAGE_APPS_TITLE", DescriptionLocalizationKey = "GIZ_MODULE_PAGE_APPS_DESCRIPTION")]
    [DefaultRoute(ClientRoutes.ApplicationsRoute), Route(ClientRoutes.ApplicationsRoute)]
    public partial class AppsIndex : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Inject]
        IOptions<PopularItemsOptions> PopularItemsOptions { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        AppsPageViewService AppsPageService { get; set; }

        [Inject()]
        public AppsPageViewState ViewState { get; set; }

        [Inject]
        AdvertisementsViewService AdvertisementsViewStateService { get; set; }

        [Inject]
        AdvertisementsViewState AdvertisementsViewState { get; set; }

        #region PARAMETERS

        /// <summary>
        /// Application id url parameter.
        /// </summary>
        [Parameter()]
        public int? AppId { get; init; }

        #endregion

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InvokeVoidAsync("registerAdsAutoCollapse");
                await InvokeVoidAsync("registerAppsSticky");
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
            await InvokeVoidAsync("unregisterAppsSticky", Ref).ConfigureAwait(false);
            await InvokeVoidAsync("unregisterAdsAutoCollapse", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}
