using System;
using System.Threading.Tasks;
using Gizmo.UI;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class GizNotification : CustomDOMComponentBase
    {
        [Parameter]
        public AlertTypes Icon { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter()]
        public string Title
        {
            get; set;
        }

        [Parameter()]
        public string Message
        {
            get; set;
        }

        [Parameter()]
        public Type DetailComponent
        {
            get;set;
        }

        [Parameter]
        public EventCallback<int> OnClose { get; set; }

        private async Task CloseNotification()
        {
            await DismissCallback.InvokeAsync();
        }
    }
}
