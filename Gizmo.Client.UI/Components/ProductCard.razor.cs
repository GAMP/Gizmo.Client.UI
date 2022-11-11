using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductCard : CustomDOMComponentBase
    {
        private bool _clickHandled = false;

        protected bool _shouldRender;

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        public bool IsHoverable { get; set; }

        [Parameter]
        public ProductViewState Product { get; set; }

        public string GetPrice()
        {
            string result = "";

            if (Product != null)
            {
                result += Product.UnitPrice.ToString("C");

                if (Product.PurchaseOptions.HasFlag(PurchaseOptionType.Or))
                {
                    result += " or " + Product.UnitPointsPrice;
                }
                else
                {
                    result += " and " + Product.UnitPointsPrice;
                }
            }

            return result;
        }

        public void OpenDetails()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            NavigationManager.NavigateTo($"/productdetails/{Product.Id}");
        }

        public async Task AddProduct()
        {
            _clickHandled = true;

            await UserCartService.AddProductAsyc(Product.Id);
        }

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<ProductViewState>(nameof(Product), out var newProduct))
            {
                var productChanged = !EqualityComparer<ProductViewState>.Default.Equals(Product, newProduct);
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
                await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}