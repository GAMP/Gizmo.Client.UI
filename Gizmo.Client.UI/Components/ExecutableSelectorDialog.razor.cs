using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableSelectorDialog : CustomDOMComponentBase
    {
        [Inject]
        AppViewStateLookupService AppViewStateLookupService { get; set; }

        [Inject]
        AppExeViewStateLookupService AppExeViewStateLookupService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public int ApplicationId { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private Task CloseDialogAsync()
        {
            return CancelCallback.InvokeAsync();
        }
    }
}
