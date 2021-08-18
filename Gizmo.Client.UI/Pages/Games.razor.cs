using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_GAMES)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_GAMES_TITLE",DescriptionLocalizationKey = "MODULE_PAGE_GAMES_TITLE"), ModuleDisplayOrder(2)]
    [Route("/games")]
    public partial class Games : ComponentBase
    {
    }
}
