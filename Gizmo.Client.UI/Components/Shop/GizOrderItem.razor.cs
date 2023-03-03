using System.Threading.Tasks;

using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrderItem : CustomDOMComponentBase
    {
        private UserProductViewState _product;

        private UserCartProductItemViewState _previousProductItemViewState;

        [Inject]
        UserCartService UserCartService { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Parameter]
        public UserCartProductItemViewState ProductItemViewState { get; set; }

        public string GetPurchaseOptionsGroup()
        {
            return "PurchaseOptions_" + ProductItemViewState.ProductId;
        }

        public Task OnRemoveQuantityButtonClickHandler(MouseEventArgs _) =>
            UserCartService.RemoveUserCartProductAsync(ProductItemViewState.ProductId);

        public Task OnAddQuantityButtonClickHandlerAsync(MouseEventArgs _) =>
            UserCartService.AddUserCartProductAsync(ProductItemViewState.ProductId);

        public Task SetPayType(bool isChecked, OrderLinePayType payType)
        {
            if (isChecked)
                return UserCartService.ChangeProductPayType(ProductItemViewState.ProductId, payType);
            else
                return Task.CompletedTask;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var orderLineChanged = _previousProductItemViewState != ProductItemViewState;

            if (orderLineChanged)
            {
                if (_previousProductItemViewState != null)
                {
                    //The same component used again with a different order line.
                    //We have to unbind from the old product.
                    this.UnsubscribeChange(_previousProductItemViewState);
                }

                _previousProductItemViewState = ProductItemViewState;

                _product = await UserProductViewStateLookupService.GetStateAsync(ProductItemViewState.ProductId);

                //We have to bind to the new product.
                this.SubscribeChange(ProductItemViewState);
            }
        }

        public override void Dispose()
        {
            if (ProductItemViewState != null)
            {
                this.UnsubscribeChange(ProductItemViewState);
            }

            base.Dispose();
        }
    }
}
