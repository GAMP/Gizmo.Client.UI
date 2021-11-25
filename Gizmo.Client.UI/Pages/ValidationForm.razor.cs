using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_SHOP)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_SHOP_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_SHOP_DESCRIPTION"), ModuleDisplayOrder(5)]
    [Route("/validation")]
    public partial class ValidationForm : ComponentBase
    {
        public ValidationModel Model { get; set; } = new ValidationModel();
    }

    public class ValidationModel
    {
        [StringLength(2, ErrorMessageResourceName = "MODULE_PAGE_APPS_TITLE",
            ErrorMessageResourceType = typeof(Resources.Properties.Resources))]
        [Required(ErrorMessageResourceName = "MODULE_PAGE_APPS_TITLE",
            ErrorMessageResourceType = typeof(Resources.Properties.Resources))]
        public string Email
        {
            get;
            set;
        }
    }
}
