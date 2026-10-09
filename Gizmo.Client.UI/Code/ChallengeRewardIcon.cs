using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;

namespace Gizmo.Client.UI
{
    public static class ChallengeRewardIcon
    {
        public static string Glyph(UserChallengeRewardViewState reward) => reward?.Kind switch
        {
            ChallengeRewardKind.Points => "ph-fill ph-coins",
            ChallengeRewardKind.Time => "ph-fill ph-clock",
            _ => "ph-fill ph-gift",
        };

        public static bool IsPoints(UserChallengeRewardViewState reward) => reward?.Kind == ChallengeRewardKind.Points;
    }
}
