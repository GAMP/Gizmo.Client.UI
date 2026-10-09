using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class SignupExpired : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter]
        public EventCallback OnVerifyAgain { get; set; }
    }
}
