using Gizmo.Client.UI.View.Services;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using Gizmo.UI.Services;
using System.Reflection.Metadata.Ecma335;

namespace Gizmo.Client.UI.Pages
{
    public partial class ProductsProductProperties : CustomDOMComponentBase
    {
        private UserProductViewState _userProductViewState;

        [Inject]
        public UserProductViewState UserProductViewState
        {
            get { return _userProductViewState; }
            private set { _userProductViewState = value; }
        }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public DateTime? PurchaseTime { get; set; }

        [Parameter]
        public DateTime? FirstUsageTime { get; set; }

        private string GetTime()
        {
            string result = string.Empty;

            if (_userProductViewState != null && _userProductViewState.ProductType == ProductType.ProductTime)
            {
                if (_userProductViewState.TimeProduct.ExpirationOptions.HasFlag(ProductTimeExpirationOptionType.ExpireAfterTime))
                {
                    string time = string.Empty;

                    switch (_userProductViewState.TimeProduct.ExpireAfterType)
                    {
                        case ExpireAfterType.Day:
                            time = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_DAYS_ABBREVIATED");
                            break;
                        case ExpireAfterType.Hour:
                            time = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_HOURS_ABBREVIATED");
                            break;
                        case ExpireAfterType.Minute:
                            time = LocalizationService.GetString("GIZ_PRODUCT_TIME_EXPIRATION_MINUTES_ABBREVIATED");
                            break;
                    }

                    result = $"{_userProductViewState.TimeProduct.ExpiresAfter} {time}";
                }
                if (_userProductViewState.TimeProduct.ExpirationOptions.HasFlag(ProductTimeExpirationOptionType.ExpireAtDayTime))
                {
                    DateTime dateTime = new DateTime();
                    dateTime = dateTime.AddMinutes(_userProductViewState.TimeProduct.ExpireAtDayTimeMinute);
                    result = $"{dateTime.ToLongTimeString()}";
                }
            }

            return result;
        }

        protected override async Task OnInitializedAsync()
        {
            _userProductViewState = await UserProductViewStateLookupService.GetStateAsync(ProductId);

            if (_userProductViewState != null)
            {
                this.SubscribeChange(_userProductViewState);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_userProductViewState != null)
            {
                this.UnsubscribeChange(_userProductViewState);
            }

            base.Dispose();
        }
    }
}
