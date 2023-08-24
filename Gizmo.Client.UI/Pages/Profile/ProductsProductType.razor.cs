using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    public partial class ProductsProductType : CustomDOMComponentBase
    {
        [Parameter]
        public TimeProductViewState TimeProduct { get; set; }
    }
}
