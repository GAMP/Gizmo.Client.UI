using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI
{
    public static class ProductHelpers
    {
        private const int SECONDS_PER_DAY = 86400;

        public static string GetProductTimeImage(UserProductViewState product)
        {
            string result = "product-time-default-1.svg";

            if (product != null)
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes(product.TimeProduct.Minutes);

                if (timeSpan.TotalHours >= 1 && timeSpan.TotalHours < 25)
                {
                    result = $"product-time-default-{((int)timeSpan.TotalHours)}.svg";
                }
                else if (timeSpan.TotalHours > 24)
                {
                    result = "product-time-default-24.svg";
                }
            }

            return result;
        }

        public static string GetProductTimeNumber(UserProductViewState product)
        {
            string result = string.Empty;

            if (product != null)
            {
                if (product.TimeProduct.Minutes < 60)
                {
                    result = product.TimeProduct.Minutes.ToString();
                }
                else
                {
                    TimeSpan timeSpan = TimeSpan.FromMinutes(product.TimeProduct.Minutes);
                    if (timeSpan.TotalHours > 24)
                    {
                        result = "24+";
                    }
                    else
                    {
                        result = ((int)timeSpan.TotalHours).ToString();

                        if (timeSpan.Minutes > 0)
                        {
                            result += "+";
                        }
                    }
                }
            }

            return result;
        }

        public static string GetProductTimeText(UserProductViewState product, ILocalizationService localizationService)
        {
            string result = string.Empty;

            if (product != null)
            {
                if (product.TimeProduct.Minutes < 60)
                {
                    result = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_TIME_PRODUCT_MINUTES));
                }
                else
                {
                    if (product.TimeProduct.Minutes >= 60 && product.TimeProduct.Minutes < 120)
                    {
                        result = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_TIME_PRODUCT_HOUR));
                    }
                    else
                    {
                        result = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_TIME_PRODUCT_HOURS));
                    }
                }
            }

            return result;
        }

        public static string GetDayTimeFromMinute(int minute)
        {
            TimeSpan timeSpan = TimeSpan.FromMinutes(minute);
            return timeSpan.ToString("hh\\:mm");
        }

        public static string GetExpiresAfterText(UserProductTimeViewState timeProduct, ILocalizationService localizationService)
        {
            string result = string.Empty;

            if (timeProduct != null)
            {
                string expireAfterText = string.Empty;

                switch (timeProduct.ExpireAfterType)
                {
                    case Web.Api.Models.ExpireAfterType.Day:

                        if (timeProduct.ExpiresAfter == 1)
                            expireAfterText = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_DAY_ABBREVIATED));
                        else
                            expireAfterText = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_DAYS_ABBREVIATED));

                        break;

                    case Web.Api.Models.ExpireAfterType.Hour:

                        if (timeProduct.ExpiresAfter == 1)
                            expireAfterText = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_HOUR_ABBREVIATED));
                        else
                            expireAfterText = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_HOURS_ABBREVIATED));

                        break;

                    case Web.Api.Models.ExpireAfterType.Minute:

                        if (timeProduct.ExpiresAfter == 1)
                            expireAfterText = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_MINUTE_ABBREVIATED));
                        else
                            expireAfterText = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_MINUTES_ABBREVIATED));

                        break;
                }

                string expireFromOptionsText = "";

                switch (timeProduct.ExpireFromOptions)
                {
                    case Web.Api.Models.ExpireFromOptionType.Purchase:

                        expireFromOptionsText = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_AFTER_PURCHASE));

                        break;

                    case Web.Api.Models.ExpireFromOptionType.Use:

                        expireFromOptionsText = localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_AFTER_USE));
                        break;
                }

                result = $"{timeProduct.ExpiresAfter} {expireAfterText} {expireFromOptionsText}";
            }

            return result;
        }

        public static List<string> GetPurchaseAvailabilities(UserProductViewState product, bool firstOnly, ILocalizationService localizationService)
        {
            return GetAvailabilities(product.PurchaseAvailability, firstOnly, localizationService);
        }

        public static List<string> GetUsageAvailabilities(UserProductViewState product, bool firstOnly, ILocalizationService localizationService)
        {
            return GetAvailabilities(product.TimeProduct.UsageAvailability, firstOnly, localizationService);
        }

        public static List<string> GetAvailabilities(ProductAvailabilityViewState availability, bool firstOnly, ILocalizationService localizationService)
        {
            List<string> result = new List<string>();

            if (availability == null)
                return result;

            if (availability.TimeRange && availability.DaysAvailable.Count() == 0)
            {
                result.Add(localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_NEVER)));
                return result;
            }

            var now = DateTime.Now;
            DateTime? lastTimeRangeEnd = null;

            if (availability.DateRange && availability.EndDate.HasValue && availability.TimeRange)
            {
                //If product has both date range and time range we need to find the last time range before the date range expiration.
                int days = 7; //We need to scan max 7 days (0-6).
                if (availability.StartDate.HasValue)
                {
                    //But if the product has start date then maybe it's less than 7 days.
                    days = Math.Min((int)availability.EndDate.Value.AddDays(1).Subtract(availability.StartDate.Value).TotalDays, days);
                }
                for (int i = 0; i < days; i++) //Loop in reverse from the end date to find the last availabe time range.
                {
                    var date = availability.EndDate.Value.AddDays(i * -1);
                    var lastEndDate = availability.DaysAvailable.Where(a => a.Day == date.DayOfWeek).FirstOrDefault();
                    if (lastEndDate != null && lastEndDate.DayTimesAvailable != null)
                    {
                        var lastEndSecond = lastEndDate.DayTimesAvailable.OrderByDescending(a => a.EndSecond).Select(a => a.EndSecond).FirstOrDefault();
                        lastTimeRangeEnd = date.Date.AddSeconds(lastEndSecond);
                        break;
                    }
                }
            }

            bool startsMoreThanWeekLater = availability.StartDate.HasValue && availability.StartDate.Value.AddDays(-6) > now;

            //It's expired if current date is greater than the end date or the last time range.
            bool expired = (availability.EndDate.HasValue && availability.EndDate.Value.AddDays(1) < now) || (lastTimeRangeEnd.HasValue && lastTimeRangeEnd.Value < now);
            bool showDateRange = !expired && availability.DateRange && (startsMoreThanWeekLater || !availability.TimeRange);
            bool showTimeRange = !expired && availability.TimeRange && !showDateRange;

            if (expired)
            {
                result.Add(localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_NOT_AVAILABLE_ANYMORE)));
            }
            else if (showDateRange)
            {
                if (availability.StartDate.HasValue && availability.EndDate.HasValue)
                {
                    result.Add($"{availability.StartDate.Value.ToShortDateString()}-{availability.EndDate.Value.ToShortDateString()}");
                }
                else if (!availability.StartDate.HasValue && !availability.EndDate.HasValue)
                {
                    result.Add("Always"); //Normalized in service, should not exist.
                }
                else
                {
                    if (availability.StartDate.HasValue)
                    {
                        result.Add($"{localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_FROM))} {availability.StartDate.Value.ToShortDateString()}");
                    }
                    else
                    {
                        result.Add($"{localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_UNTIL))} {availability.EndDate.Value.ToShortDateString()}");
                    }
                }
            }
            else if (showTimeRange)
            {
                var mergedRanges = MergeDayTimeRanges(availability.DaysAvailable);

                //Keep a full week as seven whole days so its end cannot be confused with its start.
                if (mergedRanges.Any(a => GetRelativeEndSecond(a) - a.StartSecond == 7 * SECONDS_PER_DAY))
                {
                    mergedRanges = Enumerable.Range(0, 7).Select(day => new MergedDayTimeRange()
                    {
                        StartDay = (DayOfWeek)day,
                        StartSecond = 0,
                        EndDay = (DayOfWeek)((day + 1) % 7),
                        EndSecond = 0
                    }).ToList();
                }

                var reference = now;
                if (availability.DateRange && availability.StartDate.HasValue && availability.StartDate.Value > reference)
                    reference = availability.StartDate.Value;

                var availableRanges = new List<(DateTime Start, DateTime End)>();
                foreach (var range in mergedRanges)
                {
                    //Start with the most recent occurrence, which may still be active after midnight.
                    int daysSinceStart = ((int)reference.DayOfWeek - (int)range.StartDay + 7) % 7;
                    var startDay = reference.Date.AddDays(-daysSinceStart);
                    var start = startDay.AddSeconds(range.StartSecond);
                    var end = startDay.AddSeconds(GetRelativeEndSecond(range));

                    if (start > reference)
                    {
                        start = start.AddDays(-7);
                        end = end.AddDays(-7);
                    }
                    if (end <= reference)
                    {
                        start = start.AddDays(7);
                        end = end.AddDays(7);
                    }

                    //The date range limits the occurrence, even if its weekly range starts earlier.
                    if (availability.DateRange)
                    {
                        if (availability.StartDate.HasValue && start < availability.StartDate.Value)
                            start = availability.StartDate.Value;
                        if (availability.EndDate.HasValue && end > availability.EndDate.Value.Date.AddDays(1))
                            end = availability.EndDate.Value.Date.AddDays(1);
                    }

                    if (start < end && end > reference)
                        availableRanges.Add((start, end));
                }

                var orderedRanges = availableRanges.OrderBy(a => a.Start).ThenBy(a => a.End);
                foreach (var range in firstOnly ? orderedRanges.Take(1) : orderedRanges)
                {
                    result.Add(GetDayTimeRangeText(range.Start, range.End));
                }
            }

            return result;
        }

        /// <summary>
        /// Merges the availability fragments, that are stored split per day, back into continuous ranges.
        /// A range that crosses midnight is returned once, under the day it starts on.
        /// </summary>
        private static List<MergedDayTimeRange> MergeDayTimeRanges(IEnumerable<ProductAvailabilityDayViewState> daysAvailable)
        {
            var tmp = new List<MergedDayTimeRange>();

            foreach (var item in daysAvailable)
            {
                if (item.DayTimesAvailable == null)
                    continue;

                tmp.AddRange(item.DayTimesAvailable.Select(a => new MergedDayTimeRange()
                {
                    StartDay = item.Day,
                    StartSecond = a.StartSecond,
                    EndDay = item.Day,
                    EndSecond = a.EndSecond
                }));
            }

            var result = new List<MergedDayTimeRange>();

            for (int i = 0; i < 7; i++)
            {
                var currentDayRecords = tmp.Where(a => a.StartDay == (DayOfWeek)i).OrderBy(a => a.EndSecond).ToList();

                if (currentDayRecords.Count == 0)
                    continue;

                result.AddRange(currentDayRecords);

                foreach (var item in currentDayRecords)
                {
                    tmp.Remove(item);
                }

                if (i == 6)
                    continue;

                var currentDayClose = result.Where(a => a.StartDay == (DayOfWeek)i && a.EndSecond == SECONDS_PER_DAY).FirstOrDefault();
                if (currentDayClose == null)
                    continue;

                bool done = false;
                int nextDay = i + 1;

                while (!done)
                {
                    //The range continues if the next day opens at midnight.
                    var merge = tmp.Where(a => a.StartDay == (DayOfWeek)nextDay && a.StartSecond == 0).FirstOrDefault();
                    if (merge == null)
                    {
                        done = true;
                        continue;
                    }

                    currentDayClose.EndDay = merge.EndDay;
                    currentDayClose.EndSecond = merge.EndSecond;

                    tmp.Remove(merge);

                    if (merge.EndSecond != SECONDS_PER_DAY || nextDay == 6)
                        done = true;
                    else
                        nextDay += 1;
                }
            }

            //A range that closes a day ends at midnight of the next one.
            foreach (var item in result.Where(a => a.EndSecond == SECONDS_PER_DAY))
            {
                item.EndDay = (int)item.EndDay < 6 ? item.EndDay + 1 : DayOfWeek.Sunday;
                item.EndSecond = 0;
            }

            //The range that closes the week continues into the one that opens it.
            var firstDay = result.Where(a => a.StartDay == DayOfWeek.Sunday && a.StartSecond == 0).FirstOrDefault();
            var lastDay = result.Where(a => a.EndDay == DayOfWeek.Sunday && a.EndSecond == 0).FirstOrDefault();

            if (firstDay != null && lastDay != null && result.Count > 1)
            {
                lastDay.EndDay = firstDay.EndDay;
                lastDay.EndSecond = firstDay.EndSecond;

                result.Remove(firstDay);
            }

            return result;
        }

        /// <summary>
        /// Gets the end of the range in seconds counted from the start of the day it starts on, so that
        /// ranges crossing midnight compare after the ones that end on the same day.
        /// </summary>
        private static int GetRelativeEndSecond(MergedDayTimeRange range)
        {
            int endSecond = ((((int)range.EndDay - (int)range.StartDay) + 7) % 7 * SECONDS_PER_DAY) + range.EndSecond;
            return endSecond <= range.StartSecond ? endSecond + 7 * SECONDS_PER_DAY : endSecond;
        }

        private static string GetDayTimeRangeText(DateTime start, DateTime end)
        {
            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;

            return $"{dateTimeFormat.GetShortestDayName(start.DayOfWeek)} {start.ToString("HH:mm")} - {dateTimeFormat.GetShortestDayName(end.DayOfWeek)} {end.ToString("HH:mm")}";
        }

        private sealed class MergedDayTimeRange
        {
            public DayOfWeek StartDay { get; set; }

            public int StartSecond { get; set; }

            public DayOfWeek EndDay { get; set; }

            public int EndSecond { get; set; }
        }
    }
}
