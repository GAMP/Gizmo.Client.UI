using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserBalance : ComponentBase
    {
        private bool _hideBalance;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserService UserService { get; set; }

        private void ToggleBalanceVisibility()
        {
            _hideBalance = !_hideBalance;
        }
    }
}
