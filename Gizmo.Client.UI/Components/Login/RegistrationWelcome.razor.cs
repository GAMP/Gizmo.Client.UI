using Gizmo.Client.UI.Localization.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class RegistrationWelcome : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter]
        public PlayerCardData Card { get; set; }

        [Parameter]
        public EventCallback OnDone { get; set; }
    }
}
