using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class PaymentDialog : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private Task CloseDialog()
        {
            return CancelCallback.InvokeAsync();
        }
    }
}
