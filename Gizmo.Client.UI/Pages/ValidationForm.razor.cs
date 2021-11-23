using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_SHOP)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_SHOP_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_SHOP_DESCRIPTION"), ModuleDisplayOrder(5)]
    [Route("/validation")]
    public partial class ValidationForm
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
