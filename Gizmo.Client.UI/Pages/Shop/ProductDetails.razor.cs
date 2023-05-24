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

        [Parameter]
        [SupplyParameterFromQuery]
        public int ProductId { get; set; }

        #endregion

        private Task OnClickBackButtonHandler()
        {
            return NavigationService.GoBackAsync();
        }

        private string GetDayTimeFromMinute()
        {
            TimeSpan timeSpan = TimeSpan.FromMinutes(ViewState.Product.TimeProduct.ExpireAtDayTimeMinute);
            return timeSpan.ToString("hh\\:mm");
        }

        private string GetAfterText()
        {
            string expireAfterText = "";

            switch (ViewState.Product.TimeProduct.ExpireAfterType)
            {
                case ExpireAfterType.Day:

                    if (ViewState.Product.TimeProduct.ExpiresAfter == 1)
                        expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_DAY");
                    else
                        expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_DAYS");

                    break;

                case ExpireAfterType.Hour:

                    if (ViewState.Product.TimeProduct.ExpiresAfter == 1)
                        expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_HOUR");
                    else
                        expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_HOURS");

                    break;

                case ExpireAfterType.Minute:

                    if (ViewState.Product.TimeProduct.ExpiresAfter == 1)
                        expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_MINUTE");
                    else
                        expireAfterText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_MINUTES");

                    break;
            }

            string expireFromOptionsText = "";

            switch (ViewState.Product.TimeProduct.ExpireFromOptions)
            {
                case ExpireFromOptionType.Purchase:

                    expireFromOptionsText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER_PURCHASE");

                    break;

                case ExpireFromOptionType.Use:

                    expireFromOptionsText = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_AFTER_USE");

                    break;
            }

            return $"{ViewState.Product.TimeProduct.ExpiresAfter} {expireAfterText} {LocalizationService.GetString("GIN_GEN_OF")} {expireFromOptionsText}"; //TODO: AAA
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
            _hostGroups = await UserHostGroupViewStateLookupService.GetStatesAsync();

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
