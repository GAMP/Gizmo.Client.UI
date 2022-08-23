using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
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
        HomePageService HomePageService { get; set; }

        public bool ExecutableSelectorIsOpen { get; set; }

        #endregion

        #region METHODS

        public void OpenExecutableSelector(int id)
        {
            ExecutableSelectorIsOpen = true;
        }

        #endregion
    }
}