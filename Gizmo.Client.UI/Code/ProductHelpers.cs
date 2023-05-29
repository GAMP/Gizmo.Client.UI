using System;
using System.Collections.Generic;
using System.Linq;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI
{
    public static class ProductHelpers
    {
        public static string GetProductTimeImage(UserProductViewState product)
        {
            string result = "product-time-default-1.svg";

            if (product != null)
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes(product.TimeProduct.Minutes);

                if (timeSpan.Hours > 12)
                {
                    result = "product-time-default-24.svg";
                }
                else if (timeSpan.Hours > 6)
                {
                    result = "product-time-default-11.svg";
                }
                else if (timeSpan.Hours > 3)
                {
                    result = "product-time-default-4.svg";
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
                    result = timeSpan.Hours.ToString();

                    if (timeSpan.Minutes > 0)
                    {
                        result += "+";
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
                    result = localizationService.GetString("GIZ_TIME_PRODUCT_MINUTES");
                }
                else
                {
                    if (product.TimeProduct.Minutes >= 60 && product.TimeProduct.Minutes < 120)
                    {
                        result = localizationService.GetString("GIZ_TIME_PRODUCT_HOUR");
                    }
                    else
                    {
                        result = localizationService.GetString("GIZ_TIME_PRODUCT_HOURS");
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
                    case ExpireAfterType.Day:

                        if (timeProduct.ExpiresAfter == 1)
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_DAY_ABBREVIATED");
                        else
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_DAYS_ABBREVIATED");

                        break;

                    case ExpireAfterType.Hour:

                        if (timeProduct.ExpiresAfter == 1)
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_HOUR_ABBREVIATED");
                        else
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_HOURS_ABBREVIATED");

                        break;

                    case ExpireAfterType.Minute:

                        if (timeProduct.ExpiresAfter == 1)
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_MINUTE_ABBREVIATED");
                        else
                            expireAfterText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_MINUTES_ABBREVIATED");

                        break;
                }

                string expireFromOptionsText = "";

                switch (timeProduct.ExpireFromOptions)
                {
                    case ExpireFromOptionType.Purchase:

                        expireFromOptionsText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER_PURCHASE");

                        break;

                    case ExpireFromOptionType.Use:

                        expireFromOptionsText = localizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER_USE");
                        break;
                }

                result = $"{timeProduct.ExpiresAfter} {expireAfterText} {localizationService.GetString("GIN_GEN_OF")} {expireFromOptionsText}";
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

            DateTime? lastTimeRangeEnd = null;

            if (availability.DateRange && availability.EndDate.HasValue && availability.TimeRange)
            {
                int days = 6;
                if (availability.StartDate.HasValue)
                {
                    days = Math.Min((int)availability.EndDate.Value.Subtract(availability.StartDate.Value).TotalDays, days);
                }
                for (int i = 0; i < days; i++) //TODO: AAA CHECK
                {
                    var date = availability.EndDate.Value.AddDays(i * -1);
                    var lastEndDate = availability.DaysAvailable.Where(a => a.Day == date.DayOfWeek).FirstOrDefault();
                    if (lastEndDate != null && lastEndDate.DayTimesAvailable != null)
                    {
                        var lastEndSecond = lastEndDate.DayTimesAvailable.OrderByDescending(a => a.EndSecond).Select(a => a.EndSecond).FirstOrDefault();
                        TimeSpan endTimeSpan = TimeSpan.FromSeconds(lastEndSecond);
                        lastTimeRangeEnd = new DateTime(date.Year, date.Month, date.Day, endTimeSpan.Hours, endTimeSpan.Minutes, endTimeSpan.Seconds);
                        break;
                    }
                }
            }

            bool moreThanWeekLater = availability.StartDate.HasValue && availability.StartDate.Value.AddDays(-6) > DateTime.Now;
            bool expired = (availability.EndDate.HasValue && availability.EndDate.Value.AddDays(1) < DateTime.Now) || (lastTimeRangeEnd.HasValue && lastTimeRangeEnd.Value < DateTime.Now);
            bool showDateRange = availability.DateRange && (moreThanWeekLater || expired || !availability.TimeRange);
            bool showTimeRange = availability.TimeRange && !showDateRange;

            if (showDateRange)
            {
                if (expired)
                {
                    result.Add("Not available anymore");//TODO: AAA TRANSLATE
                }
                else if (availability.StartDate.HasValue && availability.EndDate.HasValue)
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
                        result.Add($"From {availability.StartDate.Value.ToShortDateString()}"); //TODO: AAA TRANSLATE
                    }
                    else
                    {
                        result.Add($"Until {availability.EndDate.Value.ToShortDateString()}"); //TODO: AAA TRANSLATE
                    }
                }
            }
            if (showTimeRange)
            {
                if (firstOnly)
                {
                    DateTime max = DateTime.Now;

                    if (availability.StartDate.HasValue && availability.StartDate > max)
                    {
                        availability.StartDate = availability.StartDate.Value;
                    }

                    int days = 6;
                    if (availability.EndDate.HasValue)
                    {
                        days = Math.Min((int)availability.EndDate.Value.Subtract(max).TotalDays, days);
                    }
                    for (int i = 0; i < days; i++) //TODO: AAA CHECK
                    {
                        max = max.AddDays(i);
                        TimeSpan timeSpan = new TimeSpan(max.Hour, max.Minute, max.Second);
                        var firstStartDate = availability.DaysAvailable.Where(a => a.Day == max.DayOfWeek).FirstOrDefault();
                        if (firstStartDate != null)
                        {
                            var firstTimeRange = firstStartDate.DayTimesAvailable.Where(b => b.EndSecond > timeSpan.TotalSeconds).OrderBy(a => a.EndSecond).FirstOrDefault();
                            if (firstTimeRange == null)
                            {
                                firstTimeRange = firstStartDate.DayTimesAvailable.OrderBy(a => a.EndSecond).FirstOrDefault();
                            }
                            if (firstTimeRange != null)
                            {
                                TimeSpan startTimeSpan = TimeSpan.FromSeconds(firstTimeRange.StartSecond);
                                TimeSpan endTimeSpan = TimeSpan.FromSeconds(firstTimeRange.EndSecond);

                                //TODO: AAA
                                result.Add($"{startTimeSpan.ToString("hh\\:mm")}-{endTimeSpan.ToString("hh\\:mm")} {max.DayOfWeek.ToString().Substring(0, 2)}");
                                break;
                            }
                        }
                        max = new DateTime(max.Year, max.Month, max.Day);
                    }
                }
                else
                {
                    List<DayOfWeek> includeDays = new List<DayOfWeek>();
                    if (availability.EndDate.HasValue && availability.EndDate.Value.AddDays(-6) < DateTime.Now) //TODO: AAA CHECK
                    {
                        var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        int days = (int)availability.EndDate.Value.AddDays(1).Subtract(today).TotalDays;
                        for (int i = 0; i < days; i++) //TODO: AAA CHECK
                        {
                            var date = today.AddDays(i);
                            includeDays.Add(date.DayOfWeek);
                        }
                    }
                    else
                    {
                        includeDays = new List<DayOfWeek>() { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
                    }
                    //TODO: AAA EXCLUDE PASSED DAYS
                    foreach (var day in availability.DaysAvailable.Where(a => a.DayTimesAvailable.Count() > 0 && includeDays.Contains(a.Day)))
                    {
                        ProductAvailabilityDayTimeViewState first = null;

                        if (day.Day == DateTime.Now.DayOfWeek)
                        {
                            TimeSpan timeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            first = day.DayTimesAvailable.Where(a => a.EndSecond > timeSpan.TotalSeconds).FirstOrDefault();
                            if (first == null)
                            {
                                first = day.DayTimesAvailable.FirstOrDefault();
                            }
                        }
                        else
                        {
                            first = day.DayTimesAvailable.FirstOrDefault();
                        }

                        TimeSpan startTimeSpan = TimeSpan.FromSeconds(first.StartSecond);
                        TimeSpan endTimeSpan = TimeSpan.FromSeconds(first.EndSecond);

                        //TODO: AAA
                        result.Add($"{startTimeSpan.ToString("hh\\:mm")}-{endTimeSpan.ToString("hh\\:mm")} {day.Day.ToString().Substring(0, 2)}");
                    }
                }
            }

            return result;
        }
    }
}
