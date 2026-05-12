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
    [ModuleGuid(KnownModules.MODULE_HOME)]
    [PageUIModule(TitleLocalizationKey = "GIZ_MODULE_PAGE_HOME_TITLE", DescriptionLocalizationKey = "GIZ_MODULE_PAGE_HOME_TITLE"), ModuleDisplayOrder(0)]
    [Route(ClientRoutes.HomeRoute)]
    public partial class Home : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Inject]
        FeedsViewState FeedsViewState { get; set; }

        [Inject]
        IOptions<ClientInterfaceOptions> ClientInterfaceOptions { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        HomePageViewService HomePageService { get; set; }

        [Inject]
        HomePageViewState ViewState { get; set; }

        [Inject]
        AdvertisementsViewState AdvertisementsViewState { get; set; }

        #endregion

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(FeedsViewState);
            this.SubscribeChange(AdvertisementsViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(FeedsViewState);
            this.UnsubscribeChange(AdvertisementsViewState);

            base.Dispose();
        }
    }
}
