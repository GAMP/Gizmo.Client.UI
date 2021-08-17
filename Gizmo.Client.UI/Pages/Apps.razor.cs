using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [ModuleDisplayOrder(1)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_APPS_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_APPS_DESCRIPTION")]
    [DefaultRoute("/apps")]
    [Route("/apps")]
    [Route("/apps/{appId:int}")]
    public partial class Apps : ComponentBase
    {
        #region PROPERTIES

        #region PARAMETERS
        
        /// <summary>
        /// Application id url parameter.
        /// </summary>
        [Parameter()]
        public int? AppId
        {
            get;
            init;
        }  

        #endregion

        #endregion
    }
}
