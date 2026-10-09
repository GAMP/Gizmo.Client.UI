using System.Linq;
using Gizmo.Client.UI.View.States;

namespace Gizmo.Client.UI
{
    public static class PlayerProgress
    {
        public static bool Any(UserLadderSummaryViewState ladder, UserChallengesViewState challenges, UserAchievementsViewState achievements) =>
            ladder.HasLevel || challenges.Challenges.Any() || achievements.Achievements.Any();
    }
}
