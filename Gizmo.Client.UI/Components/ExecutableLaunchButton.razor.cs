using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableLaunchButton : CustomDOMComponentBase
    {
        [Parameter]
        public ExecutableViewState Executable { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public bool IsFullWidth { get; set; }

        [Parameter]
        public EventCallback OnClickMainButton { get; set; }

        private Task OnClickMainButtonHandler()
        {
            return OnClickMainButton.InvokeAsync();
        }
        
        private Task OnClickPersonalFileButtonHandler(string value)
        {
            return Task.CompletedTask;
        }
    }
}
