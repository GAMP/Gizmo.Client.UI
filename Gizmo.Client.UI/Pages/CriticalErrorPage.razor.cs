using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/criticalerror")]
    public partial class CriticalErrorPage : ComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        protected string Title => GrafitLocalization?.GetString(GrafitResourceKeys.SHELL_STATE_ERROR_TITLE) ?? "Shell error";

        protected string Text => GrafitLocalization?.GetString(GrafitResourceKeys.SHELL_STATE_ERROR_TEXT) ?? "Restart the client or contact the administrator";
    }
}
