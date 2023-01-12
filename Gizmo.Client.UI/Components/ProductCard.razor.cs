using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductCard : CustomDOMComponentBase
    {
        [Parameter]
        public ProductViewState Product { get; set; }
    }
}