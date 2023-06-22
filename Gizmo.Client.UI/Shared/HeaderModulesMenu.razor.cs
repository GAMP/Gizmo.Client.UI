using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderModulesMenu : ComponentBase
    {
        [Inject()]
        private PageModulesViewState ViewState
        {
            get;set;
        }
    }
}
