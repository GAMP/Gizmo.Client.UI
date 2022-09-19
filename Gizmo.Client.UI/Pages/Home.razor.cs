using Gizmo.Client.UI.View.Services;
using Gizmo.UI;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_HOME)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_HOME_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_HOME_TITLE"), ModuleDisplayOrder(0)]
    [Route("/home")]
    public partial class Home : CustomDOMComponentBase
    {
        public Home()
        {
        }

        #region PROPERTIES

        [Inject]
        HomePageService HomePageService { get; set; }

        [Inject]
        ExecutableSelectorService ExecutableSelectorService { get; set; }

        [Inject]
        AdvertisementsService AdvertisementsService { get; set; }

        public bool ExecutableSelectorIsOpen { get; set; }

        #endregion

        #region METHODS

        public async Task OpenExecutableSelector(int id)
        {
            await ExecutableSelectorService.LoadApplicationAsync(id);
            ExecutableSelectorIsOpen = true;
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

        public override void Dispose()
        {
            _ = InvokeVoidAsync("unregisterHomeAdsAutoCollapse", Ref);

            base.Dispose();
        }
    }
}