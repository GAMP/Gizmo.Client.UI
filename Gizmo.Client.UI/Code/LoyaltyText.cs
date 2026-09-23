using System;
using System.Globalization;

using Gizmo.Client.UI.Localization;
using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI
{
    /// <summary>
    /// The words and figures of the loyalty screens: amounts in the unit an achievement
    /// is measured in, dates, perks, rewards. One place so the summary tile, the tab and
    /// the notifications spell things the same way.
    /// </summary>
    public static class LoyaltyText
    {
        private static CultureInfo Culture => CultureInfo.CurrentCulture;

        /// <summary>"1 489 очков", with the right plural form.</summary>
        public static string Points(decimal value) =>
            ShellStringOverrides.GetPlural(ShellStringOverrides.LOYALTY_POINTS_COUNT, (int)Math.Round(value));

        /// <summary>A measured value in its unit: "3", "85,00 ₽", "4 ч 30 мин", "260 очков", "5 дн.".</summary>
        public static string Amount(decimal value, SignalUnit? unit)
        {
            switch (unit)
            {
                case SignalUnit.Currency:
                    return value.ToString("C", Culture);
                case SignalUnit.Duration:
                    return Duration(TimeSpan.FromSeconds((double)value));
                case SignalUnit.Points:
                    return Points(value);
                case SignalUnit.Days:
                    return ShellStringOverrides.GetPlural(ShellStringOverrides.LOYALTY_DAYS_COUNT, (int)Math.Round(value));
                default:
                    return value.ToString(value == Math.Floor(value) ? "N0" : "N1", Culture);
            }
        }

        /// <summary>"3 из 5", "85,00 ₽ из 100,00 ₽" - the current figure against the target.</summary>
        public static string Progress(decimal? current, decimal? target, SignalUnit? unit)
        {
            if (!target.HasValue || target.Value <= 0)
                return string.Empty;

            var now = Math.Min(current ?? 0, target.Value);

            //The unit is said once, on the target, unless it is a currency (which formats itself).
            return unit is SignalUnit.Currency
                ? ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_OF, Amount(now, unit), Amount(target.Value, unit))
                : ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_OF, Amount(now, null), Amount(target.Value, unit));
        }

        /// <summary>"4 ч 30 мин", "45 мин", "2 ч".</summary>
        public static string Duration(TimeSpan span)
        {
            var hours = (int)span.TotalHours;
            var minutes = span.Minutes;

            if (hours > 0 && minutes > 0)
                return ShellStringOverrides.Get(ShellStringOverrides.DURATION_HOURS, hours) + " " + ShellStringOverrides.Get(ShellStringOverrides.DURATION_MINUTES, minutes);
            if (hours > 0)
                return ShellStringOverrides.Get(ShellStringOverrides.DURATION_HOURS, hours);

            return ShellStringOverrides.Get(ShellStringOverrides.DURATION_MINUTES, Math.Max(1, minutes));
        }

        /// <summary>"1 октября" - the day, no year.</summary>
        public static string Day(DateTime local) => local.ToString("d MMMM", Culture);

        /// <summary>"22 дня", with the right plural form.</summary>
        public static string Days(int days) =>
            ShellStringOverrides.GetPlural(ShellStringOverrides.LOYALTY_DAYS_COUNT, days);

        /// <summary>"за месяц" - the ladder's period as a phrase.</summary>
        public static string Period(CalendarPeriod period) => period switch
        {
            CalendarPeriod.Day => ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_PERIOD_DAY),
            CalendarPeriod.Week => ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_PERIOD_WEEK),
            CalendarPeriod.Quarter => ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_PERIOD_QUARTER),
            CalendarPeriod.Year => ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_PERIOD_YEAR),
            _ => ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_PERIOD_MONTH),
        };

        /// <summary>A level's perk in words, or null for a kind this shell does not know.</summary>
        public static string Perk(LadderStandingPerkModel perk)
        {
            switch (perk)
            {
                case LadderStandingDiscountPerkModel discount:
                {
                    var value = discount.CalculationType == DiscountCalculationType.Percentage
                        ? discount.Value.ToString("0.##", Culture) + " %"
                        : discount.Value.ToString("C", Culture);
                    return ShellStringOverrides.Get(discount.RewardType == DiscountRewardType.Bonus
                        ? ShellStringOverrides.LOYALTY_PERK_BONUS
                        : ShellStringOverrides.LOYALTY_PERK_DISCOUNT, value);
                }
                case LadderStandingWaitingLinePerkModel:
                    return ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_PERK_QUEUE);
                default:
                    return null;
            }
        }

        /// <summary>A reward chip: "+500 очков", "+60 мин", "Подарок".</summary>
        public static string RewardPoints(int amount) => "+" + Points(amount);
        public static string RewardTime(int seconds) => "+" + Duration(TimeSpan.FromSeconds(seconds));
        public static string RewardProduct(int quantity) =>
            quantity > 1
                ? ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_REWARD_GIFT) + " ×" + quantity.ToString(Culture)
                : ShellStringOverrides.Get(ShellStringOverrides.LOYALTY_REWARD_GIFT);

        /// <summary>The level's mark when it has no emblem picture: the first letter of its name.</summary>
        public static string Mark(string levelName)
        {
            if (string.IsNullOrWhiteSpace(levelName))
                return "•";

            var first = levelName.Trim()[0];
            return char.ToUpper(first, Culture).ToString();
        }
    }

    /// <summary>
    /// The sentences about the customer's standing, in a short form (one line on a tile or
    /// a pill) and a long one (the tab). Same rules everywhere.
    /// </summary>
    public static class LoyaltyLines
    {
        private static string T(string key, params object[] args) => ShellStringOverrides.Get(key, args);

        private static string Period(Services.LoyaltySnapshot s) =>
            s.PeriodEndLocal.HasValue ? LoyaltyText.Day(s.PeriodEndLocal.Value) : string.Empty;

        /// <summary>"Ещё 1 489 очков, чтобы удержать" / "Закреплён до 1 октября" / ...</summary>
        public static string Short(Services.LoyaltySnapshot s)
        {
            if (!s.HasLadder || s.CurrentLevel is null)
                return string.Empty;

            switch (s.LadderState)
            {
                case LadderStandingState.Awaiting:
                    return T(ShellStringOverrides.LOYALTY_AWAITING_LINE, Period(s)).TrimEnd('.');

                case LadderStandingState.Secured:
                    if (s.NextLevel is not null && s.PointsToNext.HasValue)
                        return T(ShellStringOverrides.LOYALTY_NEXT_SHORT, LoyaltyText.Points(s.PointsToNext.Value), s.NextLevel.Name);
                    return T(ShellStringOverrides.LOYALTY_SECURED_SHORT, Period(s));

                default:
                    if (s.PointsToRetain.HasValue)
                        return T(ShellStringOverrides.LOYALTY_RETAIN_SHORT, LoyaltyText.Points(s.PointsToRetain.Value));
                    if (s.NextLevel is not null && s.PointsToNext.HasValue)
                        return T(ShellStringOverrides.LOYALTY_NEXT_SHORT, LoyaltyText.Points(s.PointsToNext.Value), s.NextLevel.Name);
                    if (s.NextLevel is not null)
                        return T(ShellStringOverrides.LOYALTY_NEXT_SHORT, T(ShellStringOverrides.LOYALTY_OF, (int)Math.Round(s.ProgressToNext * 100) + " %", "100 %"), s.NextLevel.Name);
                    return T(ShellStringOverrides.LOYALTY_STATE_SECURED);
            }
        }

        /// <summary>The main sentence of the level card.</summary>
        public static string Long(Services.LoyaltySnapshot s)
        {
            if (!s.HasLadder || s.CurrentLevel is null)
                return string.Empty;

            switch (s.LadderState)
            {
                case LadderStandingState.Awaiting:
                    return T(ShellStringOverrides.LOYALTY_AWAITING_LINE, Period(s));

                case LadderStandingState.Secured:
                    return T(s.NextLevel is null ? ShellStringOverrides.LOYALTY_TOP_LINE : ShellStringOverrides.LOYALTY_SECURED_LINE, Period(s));

                default:
                    if (s.PointsToRetain.HasValue)
                        return T(ShellStringOverrides.LOYALTY_RETAIN_LINE, LoyaltyText.Points(s.PointsToRetain.Value), Period(s));
                    if (s.NextLevel is not null && s.PointsToNext.HasValue)
                        return T(ShellStringOverrides.LOYALTY_NEXT_LINE, s.NextLevel.Name, LoyaltyText.Points(s.PointsToNext.Value));
                    if (s.NextLevel is not null)
                        return T(ShellStringOverrides.LOYALTY_REQUIREMENTS_LINE, s.NextLevel.Name, RequirementNames(s, s.NextLevel));
                    return T(ShellStringOverrides.LOYALTY_TOP_LINE, Period(s));
            }
        }

        /// <summary>"Следующий уровень — X: ещё N." when the main sentence did not say it; else empty.</summary>
        public static string Next(Services.LoyaltySnapshot s)
        {
            if (s.NextLevel is null)
                return string.Empty;

            //Already the main sentence when nothing has to be kept first.
            var saidAlready = s.LadderState != LadderStandingState.Awaiting
                              && s.LadderState != LadderStandingState.Secured
                              && !s.PointsToRetain.HasValue;
            if (saidAlready)
                return string.Empty;

            if (s.PointsToNext.HasValue)
                return T(ShellStringOverrides.LOYALTY_NEXT_LINE, s.NextLevel.Name, LoyaltyText.Points(s.PointsToNext.Value));

            return T(ShellStringOverrides.LOYALTY_REQUIREMENTS_LINE, s.NextLevel.Name, RequirementNames(s, s.NextLevel));
        }

        /// <summary>The state as one word for a chip.</summary>
        public static string StateWord(Services.LoyaltySnapshot s) => s.LadderState switch
        {
            LadderStandingState.Secured => T(ShellStringOverrides.LOYALTY_STATE_SECURED),
            LadderStandingState.Awaiting => T(ShellStringOverrides.LOYALTY_STATE_AWAITING),
            _ => T(ShellStringOverrides.LOYALTY_STATE_EARNING),
        };

        private static string RequirementNames(Services.LoyaltySnapshot s, LadderStandingLevelModel level)
        {
            if (level.Requirements is null || level.Requirements.Count == 0 || s.Standing?.Achievements is null)
                return string.Empty;

            var names = new System.Collections.Generic.List<string>();
            foreach (var requirement in level.Requirements)
            {
                var achievement = System.Linq.Enumerable.FirstOrDefault(s.Standing.Achievements, a => a.AchievementId == requirement.AchievementId);
                if (achievement is null)
                    continue;

                names.Add(requirement.RequiredCount > 1 ? achievement.Name + " ×" + requirement.RequiredCount : achievement.Name);
            }

            return string.Join(", ", names);
        }
    }
}
