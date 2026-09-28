using System.Globalization;
using System.Linq;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ChallengeCard : ProfileCardBase
    {
        [Parameter] public UserChallengeViewState Item { get; set; } = null!;

        private string CardClass
        {
            get
            {
                var css = "giz-challenge-card";

                if (Item.IsDone)
                    css += " giz-challenge-card--done";
                else if (Item.IsEnded)
                    css += " giz-challenge-card--off";

                return IsInfoOpen ? css + " giz-challenge-card--active" : css;
            }
        }

        private string ArtIcon => Item.IsDone ? "ph-check" : "ph-flag-checkered";

        private string WindowClass => Item.WindowIsWarning
            ? "giz-challenge-card__window giz-challenge-card__window--warning"
            : "giz-challenge-card__window";

        private string ChipClass => Item.ChipIsSuccess
            ? "giz-challenge-card__chip giz-challenge-card__chip--success"
            : "giz-challenge-card__chip";

        private UserChallengeRequirementViewState NextRequirement =>
            Item.IsDone || Item.IsEnded ? null : Item.Requirements.FirstOrDefault(a => !a.IsMet);

        private string ProgressValue => Item.ProgressPercent.ToString("0.##", CultureInfo.InvariantCulture);

        private static string SegmentClass(UserChallengeRequirementViewState requirement) => requirement.IsMet
            ? "giz-challenge-card__segment giz-challenge-card__segment--met"
            : "giz-challenge-card__segment";

        private static string RewardIcon(UserChallengeRewardViewState reward) => reward.Kind switch
        {
            ChallengeRewardKind.Points => "ph-coins",
            ChallengeRewardKind.Time => "ph-clock",
            _ => "ph-gift",
        };
    }
}
