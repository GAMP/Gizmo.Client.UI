using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProfileHeaderStatsCredit : CustomDOMComponentBase
    {
        private bool _isOpen = false;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        private void OnClickCreditInfoHandler()
        {
            _isOpen = !_isOpen;
        }
    }
}
