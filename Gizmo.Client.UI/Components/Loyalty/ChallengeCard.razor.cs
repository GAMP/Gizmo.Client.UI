using System;
using System.Linq;

using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ChallengeCard : CustomDOMComponentBase
    {
        [Parameter] public UserAchievementChallengeModel Challenge { get; set; }

        /// <summary>The snapshot the challenge came from - it knows the achievements' names.</summary>
        [Parameter] public LoyaltySnapshot Snapshot { get; set; }

        /// <summary>"до 30 сентября · 21 день", "выполнен 26 августа", or empty.</summary>
        protected string When
        {
            get
            {
                var c = Challenge;

                if (LoyaltySnapshot.IsDone(c))
                {
                    var completed = c.MyCompletions?.OrderByDescending(m => m.CompletedTime).FirstOrDefault();
                    return completed is null
                        ? ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_STATE_DONE)
                        : ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_COMPLETED_ON, LoyaltyText.Day(LoyaltySnapshot.ToLocal(completed.CompletedTime)));
                }

                if (!LoyaltySnapshot.IsOpen(c))
                    return ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_STATE_ENDED);

                if (!c.EndTime.HasValue)
                    return string.Empty;

                var end = LoyaltySnapshot.ToLocal(c.EndTime.Value);
                var days = Math.Max(0, (int)Math.Ceiling((end - DateTime.Now).TotalDays));
                return ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_UNTIL, LoyaltyText.Day(end)) + " · " + LoyaltyText.Days(days);
            }
        }

        /// <summary>The first requirement not yet met, resolved to its achievement; null when none.</summary>
        protected UserAchievementChallengeAchievementModel Remaining
        {
            get
            {
                var requirement = Challenge.Requirements?.FirstOrDefault(r => (r.CompletedCount ?? 0) < r.RequiredCount);
                if (requirement is null)
                    return null;

                return Snapshot?.ChallengeAchievements.FirstOrDefault(a => a.AchievementId == requirement.AchievementId)
                       ?? new UserAchievementChallengeAchievementModel { AchievementId = requirement.AchievementId, Name = string.Empty };
            }
        }

        /// <summary>"3 из 5" for the remaining requirement's achievement, when it is measurable.</summary>
        protected string RemainingProgress
        {
            get
            {
                var remaining = Remaining;
                if (remaining is null || remaining.TargetValue <= 0)
                    return null;

                var text = LoyaltyText.Progress(remaining.CurrentValue, remaining.TargetValue, remaining.Unit);
                return string.IsNullOrEmpty(text) ? null : text;
            }
        }
    }
}
