using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class TopUpDialog : CustomDOMComponentBase
    {
        public TopUpDialog()
        {
        }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        TopUpService TopUpService { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private Task CloseDialog()
        {
            return CancelCallback.InvokeAsync();
        }
    }
}