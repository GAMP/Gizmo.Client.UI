using System.Linq;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ChallengeCardPopup : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserAchievementsViewState Achievements { get; set; }

        [Parameter]
        public UserChallengeViewState Item { get; set; } = null!;

        private string MetText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_PROGRESS_OF,
            Item.Requirements.Count(a => a.IsMet), Item.Requirements.Count);

        private UserChallengeRequirementViewState NextStep => Item.IsDone || Item.IsEnded
            ? null
            : Item.Requirements.FirstOrDefault(a => !a.IsMet);

        private string StepClass(UserChallengeRequirementViewState requirement)
        {
            if (requirement.IsMet)
                return "giz-challenge-popup__step giz-challenge-popup__step--met";

            return requirement == NextStep
                ? "giz-challenge-popup__step giz-challenge-popup__step--next"
                : "giz-challenge-popup__step";
        }

        private int StepNumber(UserChallengeRequirementViewState requirement) =>
            Item.Requirements.TakeWhile(a => a != requirement).Count() + 1;

        private UserAchievementViewState Achievement(UserChallengeRequirementViewState requirement) =>
            Achievements.Achievements.FirstOrDefault(a => a.AchievementId == requirement.AchievementId);

        private string WhatToDo(UserChallengeRequirementViewState requirement)
        {
            var description = Achievement(requirement)?.PopupDescription;

            return string.IsNullOrWhiteSpace(description) || description.Trim() == requirement.Name?.Trim()
                ? null
                : description;
        }

        private static string RewardIcon(UserChallengeRewardViewState reward) => ChallengeRewardIcon.IsPoints(reward)
            ? ChallengeRewardIcon.Glyph(reward) + " giz-challenge-popup__prize-icon giz-challenge-popup__prize-icon--points"
            : ChallengeRewardIcon.Glyph(reward) + " giz-challenge-popup__prize-icon";

        protected override void OnInitialized()
        {
            this.SubscribeChange(Achievements);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Achievements);

            base.Dispose();
        }
    }
}
