using Gizmo.Client.UI.Localization.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ReservationNotice : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter]
        public string? Time { get; set; }

        [Parameter]
        public string? Message { get; set; }
    }
}
