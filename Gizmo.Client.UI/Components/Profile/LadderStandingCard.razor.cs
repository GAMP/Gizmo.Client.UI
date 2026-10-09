using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class LadderStandingCard : CustomDOMComponentBase
    {
        private const int DenseLevelCount = 7;

        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        UserLadderViewState ViewState { get; set; }

        [Inject]
        UserLadderSummaryViewState Summary { get; set; }

        private string StatusText => ViewState.ShowBanner
            ? null
            : ViewState.ShowStatusLine ? ViewState.StatusLineText : Summary.HeaderStatusText;

        private string PeriodLine => string.Join(" · ", new[]
        {
            ViewState.PeriodText,
            string.Join(" ", new[] { ViewState.PeriodEndsLabelText, ViewState.PeriodEndText }.Where(a => !string.IsNullOrWhiteSpace(a))),
        }.Where(a => !string.IsNullOrWhiteSpace(a)));

        private bool IsDense => ViewState.Levels.Count() > DenseLevelCount;

        private string StepsClass => IsDense ? "giz-stairs__steps giz-stairs__steps--dense" : "giz-stairs__steps";

        private string NextFill
        {
            get
            {
                if (ViewState.ShowProgress)
                    return CssValue.Percent(ViewState.ProgressPercent);

                if (ViewState.ShowSegments && ViewState.SegmentCount > 0)
                    return CssValue.Percent(100m * ViewState.SegmentsLit / ViewState.SegmentCount);

                return null;
            }
        }

        private string HereText => ViewState.ShowScore
            ? $"{GrafitLocalization.GetString(GrafitResourceKeys.SHELL_LADDER_HERE)} · {ViewState.ScoreText}"
            : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_LADDER_HERE);

        private sealed record Step(UserLadderLevelViewState Level, string Rise, string Fill, string Here, IReadOnlyList<UserLadderRequirementViewState> Requirements);

        private IReadOnlyList<Step> Steps
        {
            get
            {
                var levels = ViewState.Levels.ToList();
                var last = levels.Count > 1 ? levels.Count - 1 : 1;

                return levels
                    .Select((level, index) => new Step(
                        level,
                        (index / (decimal)last).ToString("0.###", CultureInfo.InvariantCulture),
                        level.IsNext && !ViewState.ShowRequirements ? NextFill : null,
                        level.IsCurrent ? HereText : null,
                        level.IsNext && ViewState.ShowRequirements ? ViewState.Requirements.ToList() : null))
                    .ToList();
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(Summary);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Summary);
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
