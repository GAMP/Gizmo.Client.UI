using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Gizmo.UI.Services;
using System;
using Gizmo.Client.UI.Components;

namespace Gizmo.Client.UI.Pages
{
    public partial class ProductsProductPropertiesHeaderTooltip : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [CascadingParameter]
        protected ClientPopup ClientPopup { get; set; }

        private void Close()
        {
            ClientPopup.Close();
        }
    }
}
