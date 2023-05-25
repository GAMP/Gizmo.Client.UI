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

        private string GetFirstPurchaseAvailabilityText()
        {
            if (Product != null && Product.PurchaseAvailability != null)
            {
                var currentWeekStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(((int)DateTime.Now.DayOfWeek) * -1);

                if (Product.PurchaseAvailability.DateRange &&
                    Product.PurchaseAvailability.StartDate.HasValue &&
                    Product.PurchaseAvailability.EndDate.HasValue &&
                    Product.PurchaseAvailability.StartDate.Value < currentWeekStart)
                {
                    return Product.PurchaseAvailability.StartDate.Value.ToShortDateString();
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
                var currentWeekStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(((int)DateTime.Now.DayOfWeek) * -1);

                if (Product.TimeProduct.UsageAvailability.DateRange &&
                    Product.TimeProduct.UsageAvailability.StartDate.HasValue &&
                    Product.TimeProduct.UsageAvailability.EndDate.HasValue &&
                    Product.TimeProduct.UsageAvailability.StartDate.Value < currentWeekStart)
                {
                    return Product.TimeProduct.UsageAvailability.StartDate.Value.ToShortDateString();
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
