using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearchResultCard : CustomDOMComponentBase
    {
        [Parameter]
        public SearchResultViewState Result { get; set; }
    }
}
