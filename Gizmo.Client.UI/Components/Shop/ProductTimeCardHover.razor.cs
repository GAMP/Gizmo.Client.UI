using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductTimeCardHover : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        private string GetTimeText()
        {
            if (Product != null && Product.ProductType == ProductType.ProductTime)
            {
                //TODO: AAA
                return $"{Product.TimeProduct?.Minutes.ToString("N0")} {LocalizationService.GetString("GIZ_PRODUCT_TIME_MINUTES_ABBREVIATED")}";
            }

            return string.Empty;
        }

        private string GetPurchaseAvailabilityText()
        {
            if (Product != null && Product.PurchaseAvailability != null)
            {
                if (Product.PurchaseAvailability.DateRange && Product.PurchaseAvailability.StartDate.HasValue && Product.PurchaseAvailability.EndDate.HasValue)
                {
                    //TODO: AAA
                    return $"{Product.PurchaseAvailability.StartDate.Value.ToShortDateString()}-{Product.PurchaseAvailability.EndDate.Value.ToShortDateString()}";
                }
                else
                {
                    //Try to find the next available day (including current day).
                    var dayAvailable = Product.PurchaseAvailability.DaysAvailable.Where(a => a.Day >= DateTime.Now.DayOfWeek).OrderBy(a => a.Day).FirstOrDefault();

                    if (dayAvailable == null)
                    {
                        //If not found, try to find any available day.
                        dayAvailable = Product.PurchaseAvailability.DaysAvailable.OrderBy(a => a.Day).FirstOrDefault();
                    }

                    if (dayAvailable != null)
                    {
                        ProductAvailabilityDayTimeViewState first = null;

                        if (dayAvailable.Day == DateTime.Now.DayOfWeek)
                        {
                            TimeSpan timeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            first = dayAvailable.DayTimesAvailable.Where(a => a.EndSecond > timeSpan.TotalSeconds).FirstOrDefault();
                            if (first == null)
                            {
                                first = dayAvailable.DayTimesAvailable.FirstOrDefault();
                            }
                        }
                        else
                        {
                            first = dayAvailable.DayTimesAvailable.FirstOrDefault();
                        }

                        if (first != null)
                        {
                            TimeSpan startTimeSpan = TimeSpan.FromSeconds(first.StartSecond);
                            TimeSpan endTimeSpan = TimeSpan.FromSeconds(first.EndSecond);

                            //TODO: AAA
                            return $"{startTimeSpan.ToString("hh\\:mm")}-{endTimeSpan.ToString("hh\\:mm")} {dayAvailable.Day.ToString().Substring(0, 2)}";
                        }
                    }
                }
            }

            return string.Empty;
        }

        private string GetUsageAvailabilityText()
        {
            if (Product != null && Product.ProductType == ProductType.ProductTime && Product.TimeProduct != null && Product.TimeProduct.UsageAvailability != null)
            {
                if (Product.TimeProduct.UsageAvailability.DateRange && Product.TimeProduct.UsageAvailability.StartDate.HasValue && Product.TimeProduct.UsageAvailability.EndDate.HasValue)
                {
                    //TODO: AAA
                    return $"{Product.TimeProduct.UsageAvailability.StartDate.Value.ToShortDateString()}-{Product.TimeProduct.UsageAvailability.EndDate.Value.ToShortDateString()}";
                }
                else
                {
                    //Try to find the next available day (including current day).
                    var dayAvailable = Product.TimeProduct.UsageAvailability.DaysAvailable.Where(a => a.Day >= DateTime.Now.DayOfWeek).OrderBy(a => a.Day).FirstOrDefault();

                    if (dayAvailable == null)
                    {
                        //If not found, try to find any available day.
                        dayAvailable = Product.TimeProduct.UsageAvailability.DaysAvailable.OrderBy(a => a.Day).FirstOrDefault();
                    }

                    if (dayAvailable != null)
                    {
                        ProductAvailabilityDayTimeViewState first = null;

                        if (dayAvailable.Day == DateTime.Now.DayOfWeek)
                        {
                            TimeSpan timeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            first = dayAvailable.DayTimesAvailable.Where(a => a.EndSecond > timeSpan.TotalSeconds).FirstOrDefault();
                            if (first == null)
                            {
                                first = dayAvailable.DayTimesAvailable.FirstOrDefault();
                            }
                        }
                        else
                        {
                            first = dayAvailable.DayTimesAvailable.FirstOrDefault();
                        }

                        if (first != null)
                        {
                            TimeSpan startTimeSpan = TimeSpan.FromSeconds(first.StartSecond);
                            TimeSpan endTimeSpan = TimeSpan.FromSeconds(first.EndSecond);

                            //TODO: AAA
                            return $"{startTimeSpan.ToString("hh\\:mm")}-{endTimeSpan.ToString("hh\\:mm")} {dayAvailable.Day.ToString().Substring(0, 2)}";
                        }
                    }
                }
            }

            return string.Empty;
        }
        private string GetDayTimeFromMinute()
        {
            TimeSpan timeSpan = TimeSpan.FromMinutes(Product.TimeProduct.ExpireAtDayTimeMinute);
            return $"{LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER")} {timeSpan.ToString("hh\\:mm")}";
        }

        private string GetAfterText()
        {
            if (Product != null && Product.ProductType == ProductType.ProductTime)
            {
                string expireAfterText = string.Empty;

                switch (Product.TimeProduct.ExpireAfterType)
                {
                    case ExpireAfterType.Day:

                        if (Product.TimeProduct.ExpiresAfter == 1)
                            expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_DAY");
                        else
                            expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_DAYS");

                        break;

                    case ExpireAfterType.Hour:

                        if (Product.TimeProduct.ExpiresAfter == 1)
                            expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_HOUR");
                        else
                            expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_HOURS");

                        break;

                    case ExpireAfterType.Minute:

                        if (Product.TimeProduct.ExpiresAfter == 1)
                            expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_MINUTE");
                        else
                            expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_MINUTES");

                        break;
                }

                string expireFromOptionsText = "";

                switch (Product.TimeProduct.ExpireFromOptions)
                {
                    case ExpireFromOptionType.Purchase:

                        expireFromOptionsText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER_PURCHASE");

                        break;

                    case ExpireFromOptionType.Use:

                        expireFromOptionsText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER_USE");

                        break;
                }

                return $"{LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER")} {Product.TimeProduct.ExpiresAfter} {expireAfterText} {LocalizationService.GetString("GIN_GEN_OF")} {expireFromOptionsText}";
            }

            return string.Empty;
        }

        protected override async Task OnInitializedAsync()
        {
            if (Product != null)
            {
                this.SubscribeChange(Product);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (Product != null)
            {
                this.UnsubscribeChange(Product);
            }

            base.Dispose();
        }
    }
}
