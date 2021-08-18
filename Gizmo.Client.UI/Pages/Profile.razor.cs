using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_PROFILE)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_PROFILE_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_PROFILE_TITLE"), ModuleDisplayOrder(3)]
    [Route("/profile")]
    public partial class Profile : ComponentBase
    {
    }
}
