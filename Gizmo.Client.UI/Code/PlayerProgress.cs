using System.Linq;
using Gizmo.Client.UI.View.States;

namespace Gizmo.Client.UI
{
    public static class PlayerProgress
    {
        public static bool Any(UserLadderSummaryViewState ladder, UserChallengesViewState challenges, UserAchievementsViewState achievements) =>
            ladder.HasLevel || challenges.Challenges.Any() || achievements.Achievements.Any();

        public static bool Failed(UserChallengesViewState challenges, UserAchievementsViewState achievements) =>
            (challenges.HasError && !challenges.IsLoading) || (achievements.HasError && !achievements.IsLoading);
    }
}
