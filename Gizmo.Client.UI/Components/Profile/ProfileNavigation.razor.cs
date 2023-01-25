using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProfileNavigation : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }
    }
}
