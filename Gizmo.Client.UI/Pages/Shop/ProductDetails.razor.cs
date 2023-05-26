using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.ProductDetailsRoute)]
    public partial class ProductDetails : CustomDOMComponentBase
    {
        #region FIELDS
        private UserCartProductItemViewState _productItemViewState;
        private UserProductGroupViewState _userProductGroupViewState;
        private int _previousProductId;
        private IEnumerable<UserHostGroupViewState> _hostGroups;
        #endregion

        #region PROPERTIES

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartProductItemViewStateLookupService UserCartProductItemViewStateLookupService { get; set; }

        [Inject]
        UserProductGroupViewStateLookupService UserProductGroupViewStateLookupService { get; set; }

        [Inject]
        ProductDetailsPageViewService ProductDetailsPageService { get; set; }

        [Inject()]
        UserCartProductItemViewState ProductItemViewState
        {
            get { return _productItemViewState; }
            set { _productItemViewState = value; }
        }

        [Inject()]
        UserProductGroupViewState ProductGroupViewState
        {
            get { return _userProductGroupViewState; }
            set { _userProductGroupViewState = value; }
        }

        [Inject]
        ProductDetailsPageViewState ViewState { get; set; }

        [Inject]
        UserHostGroupViewStateLookupService UserHostGroupViewStateLookupService { get; set; }

        [Inject]
        HostGroupViewState HostGroupViewState { get; set; }
        
        [Parameter]
        [SupplyParameterFromQuery]
        public int ProductId { get; set; }

        #endregion

        private Task OnClickBackButtonHandler()
        {
            return NavigationService.GoBackAsync();
        }

        public List<string> GetPurchaseAvailabilities()
        {
            return GetAvailabilities(ViewState.Product.PurchaseAvailability);
        }

        public List<string> GetUsageAvailabilities()
        {
            return GetAvailabilities(ViewState.Product.TimeProduct.UsageAvailability);
        }

        public List<string> GetAvailabilities(ProductAvailabilityViewState availability)
        {
            List<string> result = new List<string>();

            if (availability == null)
                return result;

            bool currentWeek = false; //TODO: AAA
            bool showDateRange = availability.DateRange && !currentWeek;
            bool showTimeRange = availability.TimeRange && !showDateRange;

            if (showDateRange)
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
                foreach (var day in availability.DaysAvailable.Where(a => a.DayTimesAvailable.Count() > 0))
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

                    result.Add($"{startTimeSpan.ToString("hh\\:mm")}-{endTimeSpan.ToString("hh\\:mm")} {day.Day.ToString().Substring(0, 2)}");
                }
            }

            return result;
        }

        private string GetAvailabilityText(ProductAvailabilityDayViewState productAvailabilityDayViewState)
        {
            ProductAvailabilityDayTimeViewState first = null;

            if (productAvailabilityDayViewState.Day == DateTime.Now.DayOfWeek)
            {
                TimeSpan timeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                first = productAvailabilityDayViewState.DayTimesAvailable.Where(a => a.EndSecond > timeSpan.TotalSeconds).FirstOrDefault();
                if (first == null)
                {
                    first = productAvailabilityDayViewState.DayTimesAvailable.FirstOrDefault();
                }
            }
            else
            {
                first = productAvailabilityDayViewState.DayTimesAvailable.FirstOrDefault();
            }

            if (first != null)
            {
                TimeSpan startTimeSpan = TimeSpan.FromSeconds(first.StartSecond);
                TimeSpan endTimeSpan = TimeSpan.FromSeconds(first.EndSecond);

                //TODO: AAA
                return $"{startTimeSpan.ToString("hh\\:mm")}-{endTimeSpan.ToString("hh\\:mm")} {productAvailabilityDayViewState.Day.ToString().Substring(0, 2)}";
            }

            //TODO: AAA
            return "Error";
        }

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            if (HostGroupViewState.HostGroupId.HasValue)
            {
                var hostGroups = await UserHostGroupViewStateLookupService.GetStatesAsync();
                var tmp = hostGroups.Where(a => a.Id != HostGroupViewState.HostGroupId.Value).ToList();
                var current = hostGroups.Where(a => a.Id == HostGroupViewState.HostGroupId.Value).FirstOrDefault();
                if (current != null)
                {
                    tmp.Insert(0, current);
                }
                _hostGroups = tmp;
            }
            else
            {
                _hostGroups = await UserHostGroupViewStateLookupService.GetStatesAsync();
            }

            await base.OnInitializedAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            var productChanged = _previousProductId != ProductId;

            if (productChanged)
            {
                if (_productItemViewState != null)
                {
                    //The same component used again with a different product.
                    //We have to unbind from the old product.
                    this.UnsubscribeChange(_productItemViewState);
                }

                _previousProductId = ProductId;

                _productItemViewState = await UserCartProductItemViewStateLookupService.GetStateAsync(ProductId);
                _userProductGroupViewState = await UserProductGroupViewStateLookupService.GetStateAsync(ViewState.Product.ProductGroupId); //TODO: A CHECK

                //We have to bind to the new product.
                this.SubscribeChange(_productItemViewState);
            }

            await base.OnParametersSetAsync();
        }

        public override void Dispose()
        {
            if (_productItemViewState != null)
            {
                this.UnsubscribeChange(_productItemViewState);
            }

            base.Dispose();
        }

        #endregion
    }
}
