using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearchResultCard : CustomDOMComponentBase
    {
        [Parameter]
        public SearchResultViewModel Result { get; set; }
    }
}
