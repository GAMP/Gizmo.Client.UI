using Gizmo.Client.UI.View.Services;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_HOME)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_HOME_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_HOME_TITLE"), ModuleDisplayOrder(0)]
    [Route("/home")]
    public partial class Home : ComponentBase
    {
        public Home()
        {
        }

        #region PROPERTIES

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Inject]
        HomePageService HomePageService { get; set; }

        [Inject]
        ExecutableSelectorService ExecutableSelectorService { get; set; }

        public bool ExecutableSelectorIsOpen { get; set; }

        #endregion

        #region METHODS

        public void OpenExecutableSelector(int id)
        {
            ExecutableSelectorService.SetApplication(id);
            ExecutableSelectorIsOpen = true;
        }

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("homeAdsAutoCollapse");
            }
        }
    }
}