using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class LadderLevelInfo : CustomDOMComponentBase
    {
        private const string HistoryPopupSelector = ".giz-ladder-history-wrapper";

        private bool _isInfoOpen;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public UserLadderLevelViewState Item { get; set; } = null!;

        private async Task OnInfoClick(MouseEventArgs e)
        {
            if (_isInfoOpen)
            {
                _isInfoOpen = false;
                return;
            }

            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e, HistoryPopupSelector);
            _isInfoOpen = true;
        }
    }
}
