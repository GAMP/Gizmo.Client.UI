using System.Collections.Generic;
using System.Linq;

using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductCards : CustomDOMComponentBase
    {
        [Parameter]
        public IEnumerable<UserProductViewState> Products { get; set; } = Enumerable.Empty<UserProductViewState>();
    }
}
