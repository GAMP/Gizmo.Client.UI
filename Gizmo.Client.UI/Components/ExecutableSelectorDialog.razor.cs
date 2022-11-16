using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableSelectorDialog : CustomDOMComponentBase
    {
        public ExecutableSelectorDialog()
        {
        }

        [Inject]
        ExecutableSelectorService ExecutableSelectorService { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private Task CloseDialog()
        {
            return CancelCallback.InvokeAsync();
        }
    }
}
