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

        /// <summary>
        /// Type modifier for the card. Info deliberately has none - it is
        /// the default look (brand accent), so it needs no override.
        /// </summary>
        protected string TypeModifier => Icon switch
        {
            AlertTypes.Success => "giz-notification--success",
            AlertTypes.Danger => "giz-notification--danger",
            AlertTypes.Warning => "giz-notification--warning",
            _ => string.Empty,
        };

        protected string TypeIcon => Icon switch
        {
            AlertTypes.Success => "ph-check-circle",
            AlertTypes.Danger => "ph-x-circle",
            AlertTypes.Warning => "ph-warning",
            _ => "ph-info",
        };

        private async Task CloseNotification()
        {
            await DismissCallback.InvokeAsync();
        }
    }
}
