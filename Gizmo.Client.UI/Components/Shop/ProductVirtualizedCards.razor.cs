using System.Collections.Generic;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductVirtualizedCards : CustomVirtualizedDOMComponentBase<UserProductViewState>
    {
        [Parameter]
        public IReadOnlyDictionary<int, decimal> OwnPrices { get; set; } = CartPrices.None;

        protected decimal? OwnPriceOf(UserProductViewState product) => CartPrices.Own(product, OwnPrices);
    }
}
