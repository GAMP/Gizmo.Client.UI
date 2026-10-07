using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI
{
    public static class TimeQueue
    {
        private const int DAY_SECONDS = 24 * 60 * 60;

        public static int? Minutes(string text, string format)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(format))
                return null;

            var pattern = "^" + Regex.Escape(format.Trim())
                .Replace("\\ ", "\\s*")
                .Replace("\\{0}", "(?<h>\\d+)")
                .Replace("\\{1}", "(?<m>\\d+)") + "$";

            var match = Regex.Match(text.Trim(), pattern, RegexOptions.CultureInvariant);

            if (!match.Success || !match.Groups["h"].Success || !match.Groups["m"].Success)
                return null;

            return int.Parse(match.Groups["h"].Value, CultureInfo.InvariantCulture) * 60
                + int.Parse(match.Groups["m"].Value, CultureInfo.InvariantCulture);
        }

        public static string UsableText(TimeProductViewState product) =>
            product.UsableTime?.Trim() == "-" ? product.RemainingTime : product.UsableTime;

        public static IReadOnlyList<TimeProductViewState> Queue(IEnumerable<TimeProductViewState> products) => products
            .Where(a => a.ActivationOrder.HasValue && a.TimeProductType != UsageType.Rate)
            .OrderBy(a => a.ActivationOrder)
            .ToList();

        public static int? QueuedMinutes(IEnumerable<TimeProductViewState> products, string format)
        {
            var total = 0;

            foreach (var product in Queue(products))
            {
                if (Minutes(UsableText(product), format) is not int minutes)
                    return null;

                total += minutes;
            }

            return total;
        }

        public static (TimeSpan Start, TimeSpan End)? Window(ProductAvailabilityViewState availability, DateTime now)
        {
            if (availability is null || !availability.TimeRange || availability.DaysAvailable is null)
                return null;

            var slots = Slots(availability, now.DayOfWeek);

            if (slots.Count == 0)
                return null;

            var second = (int)now.TimeOfDay.TotalSeconds;
            var slot = slots.FirstOrDefault(a => a.EndSecond > second) ?? slots[0];

            if (slot.StartSecond <= 0 && slot.EndSecond >= DAY_SECONDS - 1)
                return null;

            var end = slot.EndSecond;

            if (end >= DAY_SECONDS - 1)
            {
                var next = Slots(availability, (DayOfWeek)(((int)now.DayOfWeek + 1) % 7)).FirstOrDefault();

                if (next is not null && next.StartSecond <= 0)
                    end = next.EndSecond;
            }

            return (TimeSpan.FromSeconds(slot.StartSecond), TimeSpan.FromSeconds(end % DAY_SECONDS));
        }

        private static List<ProductAvailabilityDayTimeViewState> Slots(ProductAvailabilityViewState availability, DayOfWeek day) => availability.DaysAvailable
            .Where(a => a.Day == day)
            .SelectMany(a => a.DayTimesAvailable ?? Enumerable.Empty<ProductAvailabilityDayTimeViewState>())
            .OrderBy(a => a.StartSecond)
            .ToList();
    }
}
