using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class LadderStandingCard : CustomDOMComponentBase
    {
        private const string HistoryPopupSelector = ".giz-ladder-history-wrapper";
        private const int DenseLevelCount = 7;

        private bool _isBreakdownOpen;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserLadderViewState ViewState { get; set; }

        [Inject]
        UserLadderSummaryViewState Summary { get; set; }

        private string StatusText => ViewState.ShowBanner
            ? null
            : ViewState.ShowStatusLine ? ViewState.StatusLineText : Summary.HeaderStatusText;

        private string ProgressValue => ViewState.ProgressPercent.ToString("0.##", CultureInfo.InvariantCulture);

        private string FillClass => ViewState.ProgressIsSecured
            ? "giz-ladder-progress__fill giz-ladder-progress__fill--secured"
            : "giz-ladder-progress__fill";

        private string GoalClass => ViewState.ProgressIsSecured
            ? "giz-ladder-progress__goal giz-ladder-progress__goal--secured"
            : "giz-ladder-progress__goal";

        private string LevelsClass => ViewState.Levels.Count() > DenseLevelCount
            ? "giz-ladder-levels giz-ladder-levels--dense"
            : "giz-ladder-levels";

        private string SegmentClass(int index) => index < ViewState.SegmentsLit
            ? "giz-ladder-segments__segment giz-ladder-segments__segment--lit"
            : "giz-ladder-segments__segment";

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(Summary);

            base.OnInitialized();
        }

        private async Task OnBreakdownClick(MouseEventArgs e)
        {
            if (_isBreakdownOpen)
            {
                _isBreakdownOpen = false;
                return;
            }

            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e, HistoryPopupSelector);
            _isBreakdownOpen = true;
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Summary);
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
