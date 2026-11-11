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

        public sealed record Day(IReadOnlyList<Segment> Segments, string Hours);

        public sealed record WeekDay(string Name, bool On);

        private static readonly Day Closed = new(Array.Empty<Segment>(), string.Empty);

        public static Day Today(ProductAvailabilityViewState availability, DateTime now)
        {
            if (availability is null || !availability.TimeRange)
                return null;

            if (OutsideDates(availability, now.Date))
                return Closed;

            var times = availability.DaysAvailable
                .Where(a => a.Day == now.DayOfWeek)
                .SelectMany(OpenTimes)
                .OrderBy(a => a.StartSecond)
                .ToList();

            if (times.Count == 0)
                return Closed;

            return new Day(
                times.Select(a => new Segment(Percent(a.StartSecond / SecondsPerDay), Percent((a.EndSecond - a.StartSecond) / SecondsPerDay))).ToList(),
                string.Join(", ", times.Select(a => Clock(a.StartSecond) + "–" + Clock(a.EndSecond))));
        }

        public static IReadOnlyList<WeekDay> Week(ProductAvailabilityViewState availability, CultureInfo culture)
        {
            if (availability is null || !availability.TimeRange)
                return Array.Empty<WeekDay>();

            var open = availability.DaysAvailable
                .Where(a => OpenTimes(a).Any())
                .Select(a => a.Day)
                .ToHashSet();

            if (open.Count == 7)
                return Array.Empty<WeekDay>();

            var first = (int)culture.DateTimeFormat.FirstDayOfWeek;

            return Enumerable.Range(0, 7)
                .Select(index => (DayOfWeek)((first + index) % 7))
                .Select(day => new WeekDay(ShortName(culture, day), open.Contains(day)))
                .ToList();
        }

        private static IEnumerable<ProductAvailabilityDayTimeViewState> OpenTimes(ProductAvailabilityDayViewState day) =>
            day?.DayTimesAvailable?.Where(a => a.EndSecond > a.StartSecond) ?? Enumerable.Empty<ProductAvailabilityDayTimeViewState>();

        private static bool OutsideDates(ProductAvailabilityViewState availability, DateTime today) =>
            availability.DateRange &&
            ((availability.StartDate is DateTime start && start.Date > today) ||
             (availability.EndDate is DateTime end && end.Date < today));

        private static string ShortName(CultureInfo culture, DayOfWeek day)
        {
            var name = culture.DateTimeFormat.GetShortestDayName(day);

            return name.Length > 0 ? char.ToUpper(name[0], culture) + name[1..] : name;
        }

        private static string Clock(int second) =>
            (second / 3600).ToString("00", CultureInfo.InvariantCulture) + ":" + (second % 3600 / 60).ToString("00", CultureInfo.InvariantCulture);

        private static string Percent(double fraction) => (fraction * 100).ToString("0.###", CultureInfo.InvariantCulture) + "%";
    }
}
