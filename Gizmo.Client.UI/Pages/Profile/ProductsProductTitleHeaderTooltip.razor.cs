using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Gizmo.UI.Services;
using System;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.States;

namespace Gizmo.Client.UI.Pages
{
    public partial class ProductsProductTitleHeaderTooltip : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        CreditOptionsViewState CreditOptionsViewState { get; set; }

        [CascadingParameter]
        protected ClientPopup ClientPopup { get; set; }

        private void Close()
        {
            ClientPopup.Close();
        }
    }
}
