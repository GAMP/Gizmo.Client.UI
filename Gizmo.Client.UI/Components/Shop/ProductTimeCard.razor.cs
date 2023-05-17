using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductTimeCard : CustomDOMComponentBase
    {
        private bool _clickHandled = false;

        protected bool _shouldRender;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        public void OpenDetails()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={Product.Id}");
        }

        public void Ignore()
        {
            _clickHandled = true;
        }

        public string GetTimeNumber()
        {
            string result;

            if (Product.TimeProduct.Minutes < 60)
            {
                result = Product.TimeProduct.Minutes.ToString();
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromMinutes(Product.TimeProduct.Minutes);
                result = timeSpan.Hours.ToString();

                if (timeSpan.Minutes > 0)
                {
                    result += "+";
                }
            }

            return result;
        }

        public string GetTimeImage()
        {
            string result = "product-time-default-1.svg";

            TimeSpan timeSpan = TimeSpan.FromMinutes(Product.TimeProduct.Minutes);

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

            return result;
        }

        public string GetTimeText()
        {
            string result;

            if (Product.TimeProduct.Minutes < 60)
            {
                result = LocalizationService.GetString("GIZ_TIME_PRODUCT_MINUTES");
            }
            else
            {
                if (Product.TimeProduct.Minutes >= 60 && Product.TimeProduct.Minutes < 120)
                {
                    result = LocalizationService.GetString("GIZ_TIME_PRODUCT_HOUR");
                }
                else
                {
                    result = LocalizationService.GetString("GIZ_TIME_PRODUCT_HOURS");
                }
            }

            return result;
        }

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<UserProductViewState>(nameof(Product), out var newProduct))
            {
                var productChanged = !EqualityComparer<UserProductViewState>.Default.Equals(Product, newProduct);
                if (productChanged)
                {
                    _shouldRender = true;
                }
            }

            await base.SetParametersAsync(parameters);
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"ReRender {this}");
            }
            else
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}
