using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangeProfileDialog : CustomDOMComponentBase
    {
        public ChangeProfileDialog()
        {
        }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private async Task CloseDialog()
        {
            await CancelCallback.InvokeAsync();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
