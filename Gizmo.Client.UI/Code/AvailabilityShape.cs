using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gizmo.Client.UI.View.States;

namespace Gizmo.Client.UI
{
    public static class AvailabilityShape
    {
        private const double SecondsPerDay = 86400d;

        public sealed record Segment(string Left, string Width);

        public sealed record WeekDay(string Name, bool On);

        public static IReadOnlyList<Segment> DaySegments(ProductAvailabilityViewState availability, DayOfWeek today)
        {
            if (availability is null || !availability.TimeRange)
                return Array.Empty<Segment>();

            var day = availability.DaysAvailable.FirstOrDefault(a => a.Day == today) ?? availability.DaysAvailable.FirstOrDefault();
            if (day is null)
                return Array.Empty<Segment>();

            var times = day.DayTimesAvailable?.Where(a => a.EndSecond > a.StartSecond).ToList();
            if (times is null || times.Count == 0)
                return new[] { new Segment("0%", "100%") };

            return times
                .Select(a => new Segment(Percent(a.StartSecond / SecondsPerDay), Percent((a.EndSecond - a.StartSecond) / SecondsPerDay)))
                .ToList();
        }

        public static IReadOnlyList<WeekDay> Week(ProductAvailabilityViewState availability, CultureInfo culture)
        {
            if (availability is null || !availability.TimeRange)
                return Array.Empty<WeekDay>();

            var open = availability.DaysAvailable.Select(a => a.Day).ToHashSet();
            if (open.Count == 7)
                return Array.Empty<WeekDay>();

            var first = (int)culture.DateTimeFormat.FirstDayOfWeek;

            return Enumerable.Range(0, 7)
                .Select(index => (DayOfWeek)((first + index) % 7))
                .Select(day => new WeekDay(ShortName(culture, day), open.Contains(day)))
                .ToList();
        }

        private static string ShortName(CultureInfo culture, DayOfWeek day)
        {
            var name = culture.DateTimeFormat.GetShortestDayName(day);

            return name.Length > 0 ? char.ToUpper(name[0], culture) + name[1..] : name;
        }

        private static string Percent(double fraction) => (fraction * 100).ToString("0.###", CultureInfo.InvariantCulture) + "%";
    }
}
