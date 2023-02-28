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

        #endregion

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ShopService.ViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ShopService.ViewState);

            base.Dispose();
        }
    }
}
