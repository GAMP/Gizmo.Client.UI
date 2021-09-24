using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableCard : CustomDOMComponentBase
    {
        [Parameter]
        public ExecutableViewModel Executable { get; set; }
    }
}