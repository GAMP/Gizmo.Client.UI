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

        private bool _isBreakdownOpen;

        private decimal RingPercent
        {
            get
            {
                if (ViewState.ShowProgress)
                    return ViewState.ProgressPercent;

                if (ViewState.ShowSegments && ViewState.SegmentCount > 0)
                    return 100m * ViewState.SegmentsLit / ViewState.SegmentCount;

                return ViewState.ShowBanner ? 100m : 0m;
            }
        }

        private string RingValue => RingPercent.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserLadderViewState ViewState { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

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
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
