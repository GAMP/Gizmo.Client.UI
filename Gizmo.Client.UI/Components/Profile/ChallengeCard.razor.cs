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

        private string WindowClass => Item.WindowIsWarning
            ? "giz-challenge-card__window giz-challenge-card__window--warning"
            : "giz-challenge-card__window";

        private bool ShowStateChip => Item.HasChip && !Item.ChipIsSuccess;

        private bool ShowMeta => !string.IsNullOrEmpty(Item.WindowText) || ShowStateChip;

        private bool HasRewards => Item.Rewards.Any();

        private UserChallengeRequirementViewState NextRequirement =>
            Item.IsDone || Item.IsEnded ? null : Item.Requirements.FirstOrDefault(a => !a.IsMet);

        private string ProgressValue => CssValue.Number(Item.ProgressPercent);

        private static string SegmentClass(UserChallengeRequirementViewState requirement) => requirement.IsMet
            ? "giz-challenge-card__segment giz-challenge-card__segment--met"
            : "giz-challenge-card__segment";

        private string RequirementClass(UserChallengeRequirementViewState requirement)
        {
            if (requirement.IsMet)
                return "giz-challenge-card__step giz-challenge-card__step--met";

            return ReferenceEquals(requirement, NextRequirement)
                ? "giz-challenge-card__step giz-challenge-card__step--next"
                : "giz-challenge-card__step";
        }

        private string RequirementIcon(UserChallengeRequirementViewState requirement)
        {
            if (requirement.IsMet)
                return "ph-bold ph-check";

            return ReferenceEquals(requirement, NextRequirement) ? "ph-bold ph-arrow-right" : "ph ph-circle";
        }

        private static string RewardIcon(UserChallengeRewardViewState reward) => ChallengeRewardIcon.IsPoints(reward)
            ? ChallengeRewardIcon.Glyph(reward) + " giz-challenge-card__reward-icon giz-challenge-card__reward-icon--points"
            : ChallengeRewardIcon.Glyph(reward) + " giz-challenge-card__reward-icon";
    }
}
