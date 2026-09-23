using System;
using System.Collections.Generic;
using System.Linq;

using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Everything the shell knows about the customer's standing, read once and replaced
    /// whole by <see cref="Loyalty"/>. The derived members are what the screens ask;
    /// they are computed once here so every tile agrees.
    /// </summary>
    public sealed record LoyaltySnapshot
    {
        /// <summary>No data: a guest, a club without the feature, or before the first load.</summary>
        public static LoyaltySnapshot None { get; } = new();

        public bool IsLoading { get; init; }
        public Exception LastError { get; init; }
        public DateTime LoadedAtUtc { get; init; }

        public LadderStandingModel Standing { get; init; }
        public IReadOnlyList<UserAchievementModel> Achievements { get; init; } = Array.Empty<UserAchievementModel>();
        public IReadOnlyList<UserAchievementChallengeModel> Challenges { get; init; } = Array.Empty<UserAchievementChallengeModel>();
        public IReadOnlyList<UserAchievementChallengeAchievementModel> ChallengeAchievements { get; init; } = Array.Empty<UserAchievementChallengeAchievementModel>();

        /// <summary>Levels ordered by rank.</summary>
        public IReadOnlyList<LadderStandingLevelModel> Levels { get; init; } = Array.Empty<LadderStandingLevelModel>();
        public LadderStandingLevelModel CurrentLevel { get; init; }
        public LadderStandingLevelModel NextLevel { get; init; }

        /// <summary>0..1 towards the next level; 1 at the top.</summary>
        public double ProgressToNext { get; init; }
        /// <summary>Points still missing to keep the current level this period; null when not in question.</summary>
        public int? PointsToRetain { get; init; }
        /// <summary>Points still missing to the next level; null without a next level or in requirements mode.</summary>
        public int? PointsToNext { get; init; }

        public int EarnedAchievements { get; init; }
        public int DoneChallenges { get; init; }
        public bool RewardWaiting { get; init; }

        /// <summary>The single most useful thing to do next, or null.</summary>
        public LoyaltyAction NextAction { get; init; }

        public bool HasLadder => Levels.Count > 0;
        public bool HasAchievements => Achievements.Count > 0;
        public bool HasChallenges => Challenges.Count > 0;
        /// <summary>The club runs the system and this customer is part of it.</summary>
        public bool IsAvailable => HasLadder || HasAchievements || HasChallenges;

        public bool IsPointsLadder => Standing?.Mode == AchievementLadderMode.Points;
        public LadderStandingState? LadderState => Standing?.State;
        public decimal Score => Standing?.Score ?? 0;
        public DateTime? PeriodEndLocal => Standing is null ? null : ToLocal(Standing.PeriodEnd);
        public int DaysLeft => PeriodEndLocal.HasValue ? Math.Max(0, (int)Math.Ceiling((PeriodEndLocal.Value - DateTime.Now).TotalDays)) : 0;

        public static LoyaltySnapshot From(LadderStandingModel standing, UserAchievementsModel achievements,
            UserAchievementChallengesModel challenges, Exception error)
        {
            var levels = standing?.Levels?.OrderBy(l => l.Rank).ToList() ?? new List<LadderStandingLevelModel>();
            if (standing is not null && levels.Count == 0)
                standing = null;    //a ladder with no levels is no ladder

            var current = levels.FirstOrDefault(l => l.Rank == standing?.CurrentRank)
                          ?? levels.LastOrDefault(l => l.Rank <= (standing?.CurrentRank ?? int.MinValue))
                          ?? levels.FirstOrDefault();
            var next = current is null ? null : levels.FirstOrDefault(l => l.Rank > current.Rank);

            var achievementList = achievements?.Achievements?.ToList() ?? new List<UserAchievementModel>();
            var challengeList = challenges?.Challenges?
                .Where(c => !c.IsDeleted && !c.IsDisabled)
                .ToList() ?? new List<UserAchievementChallengeModel>();
            var challengeAchievements = challenges?.Achievements?.ToList() ?? new List<UserAchievementChallengeAchievementModel>();

            var snapshot = new LoyaltySnapshot
            {
                LoadedAtUtc = DateTime.UtcNow,
                LastError = error,
                Standing = standing,
                Achievements = achievementList,
                Challenges = challengeList,
                ChallengeAchievements = challengeAchievements,
                Levels = levels,
                CurrentLevel = current,
                NextLevel = next,
                EarnedAchievements = achievementList.Count(IsEarned),
                DoneChallenges = challengeList.Count(IsDone),
                RewardWaiting = challengeList.Any(c => c.MyCompletions?.Any(HasRewardWaiting) == true),
            };

            return snapshot with
            {
                ProgressToNext = ProgressToNextOf(snapshot),
                PointsToRetain = PointsToRetainOf(snapshot),
                PointsToNext = PointsToNextOf(snapshot),
                NextAction = NextActionOf(snapshot),
            };
        }

        #region DERIVATION

        private static double ProgressToNextOf(LoyaltySnapshot s)
        {
            if (s.Standing is null || s.CurrentLevel is null)
                return 0;

            if (s.NextLevel is null)
                return 1;

            if (s.IsPointsLadder)
            {
                var from = (decimal)(s.CurrentLevel.Threshold ?? 0);
                var to = (decimal)(s.NextLevel.Threshold ?? 0);
                if (to <= from)
                    return 1;

                return Clamp((double)((s.Score - from) / (to - from)));
            }

            //Requirements mode: the server says how far the next level's requirements are.
            return Clamp((double)Fraction(s.NextLevel.Progress));
        }

        private static int? PointsToRetainOf(LoyaltySnapshot s)
        {
            if (s.Standing is null || !s.IsPointsLadder || s.CurrentLevel?.Threshold is null)
                return null;

            if (s.Standing.State != LadderStandingState.Earning)
                return null;

            var missing = s.CurrentLevel.Threshold.Value - s.Score;
            return missing > 0 ? (int)Math.Ceiling(missing) : null;
        }

        private static int? PointsToNextOf(LoyaltySnapshot s)
        {
            if (s.Standing is null || !s.IsPointsLadder || s.NextLevel?.Threshold is null)
                return null;

            var missing = s.NextLevel.Threshold.Value - s.Score;
            return (int)Math.Ceiling(Math.Max(0, missing));
        }

        private static LoyaltyAction NextActionOf(LoyaltySnapshot s)
        {
            //A challenge one step from its reward first: it has a prize attached.
            var candidates = s.Challenges
                .Where(c => !IsDone(c) && IsOpen(c))
                .Select(c => (Challenge: c, Closeness: Closeness(c)))
                .OrderByDescending(a => a.Closeness)
                .ToList();

            foreach (var (challenge, _) in candidates)
            {
                var requirement = challenge.Requirements?
                    .FirstOrDefault(r => (r.CompletedCount ?? 0) < r.RequiredCount);
                if (requirement is null)
                    continue;

                var achievement = s.ChallengeAchievements.FirstOrDefault(a => a.AchievementId == requirement.AchievementId);
                return new LoyaltyAction(
                    LoyaltyActionKind.Challenge,
                    achievement?.Name ?? challenge.Name,
                    challenge.Name,
                    achievement?.CurrentValue, achievement?.TargetValue, achievement?.Unit,
                    challenge);
            }

            //Otherwise the achievement closest to being earned.
            var nearest = s.Achievements
                .Where(a => !IsEarned(a) && !IsSecret(a) && a.State == UserAchievementState.Active && a.TargetValue > 0)
                .OrderByDescending(a => AchievementFraction(a))
                .FirstOrDefault();

            if (nearest is null)
                return null;

            return new LoyaltyAction(LoyaltyActionKind.Achievement, nearest.Name, null,
                nearest.CurrentValue, nearest.TargetValue, nearest.Unit, null);
        }

        private static double Closeness(UserAchievementChallengeModel c)
        {
            var total = c.Requirements?.Count ?? 0;
            if (total == 0)
                return (double)Fraction(c.Progress);

            var met = c.MetCount ?? c.Requirements.Count(r => (r.CompletedCount ?? 0) >= r.RequiredCount);
            return (double)met / total;
        }

        #endregion

        #region PREDICATES

        public static bool IsEarned(UserAchievementModel a) =>
            a.State == UserAchievementState.Earned || a.InstanceCompletions > 0;

        /// <summary>Hidden and never earned: shown as a secret.</summary>
        public static bool IsSecret(UserAchievementModel a) =>
            a.IsHidden && a.TotalCompletions == 0 && !IsEarned(a);

        public static bool IsDone(UserAchievementChallengeModel c) =>
            c.State == UserAchievementChallengeState.Done
            || (c.MaxCompletions.HasValue && c.CompletionsEarned >= c.MaxCompletions.Value);

        /// <summary>Still running for this customer.</summary>
        public static bool IsOpen(UserAchievementChallengeModel c) =>
            c.State is UserAchievementChallengeState.Active or UserAchievementChallengeState.NotStarted;

        public static bool HasRewardWaiting(UserAchievementChallengeCompletionModel completion) =>
            (completion.PointsRewards?.Any(r => r.Status == AchievementChallengeRewardStatus.AwaitingClaim) ?? false)
            || (completion.ProductRewards?.Any(r => r.Status == AchievementChallengeRewardStatus.AwaitingClaim) ?? false)
            || (completion.TimeRewards?.Any(r => r.Status == AchievementChallengeRewardStatus.AwaitingClaim) ?? false);

        /// <summary>0..1 of an achievement's current period.</summary>
        public static double AchievementFraction(UserAchievementModel a)
        {
            if (IsEarned(a))
                return 1;

            if (a.TargetValue > 0 && a.CurrentValue.HasValue)
                return Clamp((double)(a.CurrentValue.Value / a.TargetValue));

            return Clamp((double)Fraction(a.Progress));
        }

        /// <summary>
        /// The server's progress figures are read as a fraction; a value above one is
        /// taken as a percentage, since the contract does not say which it is.
        /// </summary>
        public static decimal Fraction(decimal? progress)
        {
            if (!progress.HasValue)
                return 0;

            var p = progress.Value;
            if (p > 1)
                p /= 100;

            return Math.Clamp(p, 0, 1);
        }

        private static double Clamp(double value) => Math.Clamp(double.IsNaN(value) ? 0 : value, 0, 1);

        /// <summary>The server speaks UTC; unspecified kinds are taken as UTC too.</summary>
        public static DateTime ToLocal(DateTime value) =>
            value.Kind == DateTimeKind.Local ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime();

        #endregion
    }

    public enum LoyaltyActionKind
    {
        Challenge,
        Achievement,
    }

    /// <summary>
    /// One thing to do next: the achievement to work on, the challenge it serves (if any)
    /// and where the customer stands on it.
    /// </summary>
    public sealed record LoyaltyAction(
        LoyaltyActionKind Kind,
        string Title,
        string ChallengeName,
        decimal? CurrentValue,
        decimal? TargetValue,
        SignalUnit? Unit,
        UserAchievementChallengeModel Challenge);
}
