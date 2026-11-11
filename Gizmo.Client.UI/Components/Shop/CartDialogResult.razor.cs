using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class CartDialogResult : ComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter]
        public bool Ok { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Note { get; set; }

        [Parameter]
        public string Error { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public string ActionText { get; set; }

        protected string ResultClass => Ok ? "giz-buy__result giz-buy__result--ok" : "giz-buy__result giz-buy__result--error";

        protected string IconClass => Ok ? "ph-bold ph-check" : "ph-bold ph-x";

        protected string ActionLabel => !string.IsNullOrEmpty(ActionText)
            ? ActionText
            : GrafitLocalization.GetString(Ok ? GrafitResourceKeys.SHELL_GEN_DONE : nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_CLOSE));
    }
}
