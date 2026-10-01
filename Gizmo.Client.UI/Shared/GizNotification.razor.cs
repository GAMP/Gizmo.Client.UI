using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.UI;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class GizNotification : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

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

        protected string TypeModifier => Icon switch
        {
            AlertTypes.Success => "giz-notification--success",
            AlertTypes.Danger => "giz-notification--danger",
            AlertTypes.Warning => "giz-notification--warning",
            _ => string.Empty,
        };

        protected string TypeIcon => Icon switch
        {
            AlertTypes.Success => "ph-bold ph-check",
            AlertTypes.Danger => "ph-fill ph-warning-octagon",
            AlertTypes.Warning => "ph-fill ph-warning",
            _ => "ph-fill ph-chat-circle-text",
        };

        private async Task CloseNotification()
        {
            await DismissCallback.InvokeAsync();
        }
    }
}
