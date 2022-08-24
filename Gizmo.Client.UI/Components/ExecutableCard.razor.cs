using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableCard : CustomDOMComponentBase
    {
        [Parameter]
        public ExecutableViewState Executable { get; set; }

        private void OnClickMainButtonHandler()
        {

        }
    }
}