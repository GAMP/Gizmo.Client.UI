using System.Threading.Tasks;

using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductQuantityPicker : CustomDOMComponentBase
    {
        private int _previousProductId;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public ButtonSizes PikerSize { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        UserCartProductItemViewState ProductItemViewState { get; set; }

        public async Task OnAddQuantityButtonClickHandlerAsync(MouseEventArgs args)
        {
            await UserCartService.AddProductAsync(ProductId, 1);
            await OnClick.InvokeAsync(args);
        }

        public async Task OnRemoveQuantityButtonClickHandler(MouseEventArgs args)
        {
            await UserCartService.RemoveProductAsync(ProductId, 1);
            await OnClick.InvokeAsync(args);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var productIdChanged = _previousProductId != ProductId;

            if (productIdChanged)
            {
                _previousProductId = ProductId;

                if (ProductItemViewState != null)
                {
                    //The same component used again with a different product id.
                    //We have to unbind from the old product.
                    this.UnsubscribeChange(ProductItemViewState);
                }

                //We have to bind to the new product.
                ProductItemViewState = await UserCartService.GetCartProductItemViewStateAsync(ProductId);
                this.SubscribeChange(ProductItemViewState);
            }
        }

        protected override Task OnInitializedAsync()
        {
            //ProductItemViewState = await UserCartService.GetCartProductItemViewStateAsync(ProductId);

            //this.SubscribeChange(ProductItemViewState);
            return base.OnInitializedAsync();
        }
        public override void Dispose()
        {
            this.UnsubscribeChange(ProductItemViewState);
            base.Dispose();
        }
    }
}
