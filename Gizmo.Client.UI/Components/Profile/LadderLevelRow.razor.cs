using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class LadderLevelRow : CustomDOMComponentBase
    {
        private const string HistoryPopupSelector = ".giz-ladder-history-wrapper";

        private bool _isInfoOpen;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserLadderViewService Service { get; set; }

        [Parameter]
        public UserLadderLevelViewState Item { get; set; } = null!;

        private string RowClass
        {
            get
            {
                var css = "giz-ladder-level-row";

                if (Item.IsCurrent)
                    css += " giz-ladder-level-row--current";

                if (Item.IsLocked)
                    css += " giz-ladder-level-row--locked";

                return Item.IsSelected ? css + " giz-ladder-level-row--selected" : css;
            }
        }

        private string StateText
        {
            get
            {
                if (Item.IsCurrent)
                    return LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_CURRENT));

                if (Item.IsProjected)
                    return LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PROJECTED));

                if (Item.IsNext)
                    return LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_NEXT));

                return Item.IsLocked
                    ? LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_LOCKED))
                    : null;
            }
        }

        private string StateClass
        {
            get
            {
                if (Item.IsCurrent)
                    return "giz-ladder-level-row__state giz-ladder-level-row__state--current";

                if (Item.IsProjected)
                    return Item.IsProjectedDown
                        ? "giz-ladder-level-row__state giz-ladder-level-row__state--down"
                        : "giz-ladder-level-row__state";

                return Item.IsLocked
                    ? "giz-ladder-level-row__state giz-ladder-level-row__state--locked"
                    : "giz-ladder-level-row__state";
            }
        }

        private async Task OnClick(MouseEventArgs e)
        {
            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e, HistoryPopupSelector);
            Service.SelectLevel(Item.Rank);
        }

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
