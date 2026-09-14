using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class AchievementTile : CustomDOMComponentBase
    {
        [Parameter] public UserAchievementModel Achievement { get; set; }

        /// <summary>"Получено ×9", "3 из 5", "Откроется, когда получите".</summary>
        protected string Meta
        {
            get
            {
                var a = Achievement;

                if (LoyaltySnapshot.IsSecret(a))
                    return ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_SECRET_HINT);

                if (LoyaltySnapshot.IsEarned(a))
                    return a.TotalCompletions > 1
                        ? ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_EARNED_TIMES, a.TotalCompletions)
                        : ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_EARNED);

                var progress = LoyaltyText.Progress(a.CurrentValue, a.TargetValue, a.Unit);
                if (!string.IsNullOrEmpty(progress))
                    return progress;

                return a.TotalCompletions > 0
                    ? ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_EARNED_TIMES, a.TotalCompletions)
                    : string.Empty;
            }
        }
    }
}
