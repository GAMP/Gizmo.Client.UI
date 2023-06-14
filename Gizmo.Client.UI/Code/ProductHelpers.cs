using System;
using System.Collections.Generic;
using System.Linq;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

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

                if (timeSpan.TotalHours > 12)
                {
                    result = "product-time-default-24.svg";
                }
                else if (timeSpan.TotalHours > 6)
                {
                    result = "product-time-default-11.svg";
                }
                else if (timeSpan.TotalHours > 3)
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

                result = $"{timeProduct.ExpiresAfter} {expireAfterText} {localizationService.GetString("GIZ_GEN_OF")} {expireFromOptionsText}";
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
                result.Add(localizationService.GetString("GIZ_GEN_NEVER"));
                return result;
            }

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
                        TimeSpan endTimeSpan = TimeSpan.FromSeconds(lastEndSecond);
                        lastTimeRangeEnd = new DateTime(date.Year, date.Month, date.Day, endTimeSpan.Hours, endTimeSpan.Minutes, endTimeSpan.Seconds);
                        break;
                    }
                }
            }

            bool startsMoreThanWeekLater = availability.StartDate.HasValue && availability.StartDate.Value.AddDays(-6) > DateTime.Now;

            //It's expired if current date is greater than the end date or the last time range.
            bool expired = (availability.EndDate.HasValue && availability.EndDate.Value.AddDays(1) < DateTime.Now) || (lastTimeRangeEnd.HasValue && lastTimeRangeEnd.Value < DateTime.Now);
            bool showDateRange = !expired && availability.DateRange && (startsMoreThanWeekLater || !availability.TimeRange);
            bool showTimeRange = !expired && availability.TimeRange && !showDateRange;

            if (expired)
            {
                result.Add(localizationService.GetString("GIZ_PRODUCT_NOT_AVAILABLE_ANYMORE"));
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
                        result.Add($"{localizationService.GetString("GIZ_GEN_FROM")} {availability.StartDate.Value.ToShortDateString()}");
                    }
                    else
                    {
                        result.Add($"{localizationService.GetString("GIZ_GEN_UNTIL")} {availability.EndDate.Value.ToShortDateString()}");
                    }
                }
            }
            else if (showTimeRange)
            {
                if (firstOnly)
                {
                    var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    DateTime max = DateTime.Now;
                    int daysToExpiration = int.MaxValue;
                    bool repeatCurrentWeekDay = false;

                    if (availability.StartDate.HasValue && availability.StartDate > max)
                    {
                        //If the product has start date and it's greater than today then start from start date.
                        max = availability.StartDate.Value;

                        if (availability.EndDate.HasValue)
                        {
                            //But if the product has end date then maybe it's less than 7 days.
                            daysToExpiration = (int)availability.EndDate.Value.AddDays(1).Subtract(max).TotalDays;
                        }
                    }
                    else
                    {
                        if (availability.EndDate.HasValue)
                        {
                            //But if the product has end date then maybe it's less than 7 days.
                            daysToExpiration = (int)availability.EndDate.Value.AddDays(1).Subtract(today).TotalDays;
                        }

                        if (daysToExpiration > 7)
                        {
                            repeatCurrentWeekDay = true;
                        }
                    }

                    int days = 7; //We need to scan max 7 days (0-6).

                    days = Math.Min(daysToExpiration, days);

                    if (repeatCurrentWeekDay)
                    {
                        //If the end date is more than a week later then add 1 day to include the same week day of the next week in case of passed time range of current days.
                        days += 1;
                    }

                    for (int i = 0; i < days; i++)
                    {
                        var date = max.AddDays(i);
                        TimeSpan timeSpan = new TimeSpan(date.Hour, date.Minute, date.Second);
                        var firstStartDate = availability.DaysAvailable.Where(a => a.Day == date.DayOfWeek).FirstOrDefault();
                        if (firstStartDate != null)
                        {
                            var firstTimeRange = firstStartDate.DayTimesAvailable.Where(b => b.EndSecond > timeSpan.TotalSeconds).OrderBy(a => a.EndSecond).FirstOrDefault();
                            if (firstTimeRange == null && i > 0 &&  repeatCurrentWeekDay)
                            {
                                firstTimeRange = firstStartDate.DayTimesAvailable.OrderBy(a => a.EndSecond).FirstOrDefault();
                            }
                            if (firstTimeRange != null)
                            {
                                TimeSpan startTimeSpan = TimeSpan.FromSeconds(firstTimeRange.StartSecond);
                                TimeSpan endTimeSpan = TimeSpan.FromSeconds(firstTimeRange.EndSecond);

                                //TODO: AAA DAY NAME?
                                result.Add($"{startTimeSpan.ToString("hh\\:mm")}-{endTimeSpan.ToString("hh\\:mm")} {date.DayOfWeek.ToString().Substring(0, 2)}");
                                break;
                            }
                        }
                        max = new DateTime(max.Year, max.Month, max.Day); //For today we want the hour too, for the other days we don't want the hour, just the date.
                    }
                }
                else
                {
                    var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    bool expiresMoreThanWeekLater = true;

                    List<DayOfWeek> includeDays = new List<DayOfWeek>();
                    if (availability.EndDate.HasValue && availability.EndDate.Value.AddDays(-7) < today)
                    {
                        //If the product expires in less than a week we don't want to show all the days.
                        int days = (int)availability.EndDate.Value.AddDays(1).Subtract(today).TotalDays;
                        for (int i = 0; i < days; i++) //Loop from today to the end date and include only these days.
                        {
                            var date = today.AddDays(i);
                            includeDays.Add(date.DayOfWeek);
                        }
                        expiresMoreThanWeekLater = false;
                    }
                    else
                    {
                        includeDays = ((DayOfWeek[])Enum.GetValues(typeof(DayOfWeek))).ToList();
                    }

                    foreach (var day in availability.DaysAvailable.Where(a => a.DayTimesAvailable.Count() > 0 && includeDays.Contains(a.Day)))
                    {
                        ProductAvailabilityDayTimeViewState first = null;

                        if (day.Day == DateTime.Now.DayOfWeek)
                        {
                            //If current day is today find the first time range that is not passed.
                            TimeSpan timeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            first = day.DayTimesAvailable.Where(a => a.EndSecond > timeSpan.TotalSeconds).OrderBy(a => a.EndSecond).FirstOrDefault();

                            if (expiresMoreThanWeekLater && first == null)
                            {
                                //If we didn't find any time range that was not passed but the expiration is more than a week later then find the first time range.
                                first = day.DayTimesAvailable.OrderBy(a => a.EndSecond).FirstOrDefault();
                            }
                        }
                        else
                        {
                            first = day.DayTimesAvailable.OrderBy(a => a.EndSecond).FirstOrDefault();
                        }

                        if (first != null)
                        {
                            TimeSpan startTimeSpan = TimeSpan.FromSeconds(first.StartSecond);
                            TimeSpan endTimeSpan = TimeSpan.FromSeconds(first.EndSecond);

                            //TODO: AAA DAY NAME?
                            result.Add($"{startTimeSpan.ToString("hh\\:mm")}-{endTimeSpan.ToString("hh\\:mm")} {day.Day.ToString().Substring(0, 2)}");
                        }
                    }
                }
            }

            return result;
        }
    }
}
