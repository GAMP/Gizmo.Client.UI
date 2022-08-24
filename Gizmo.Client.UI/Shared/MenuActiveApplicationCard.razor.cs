using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuActiveApplicationCard : CustomDOMComponentBase
    {
        [Parameter]
        public ExecutableViewState Executable { get; set; }

        private void OnClickMainButtonHandler()
        {

        }
    }
}
