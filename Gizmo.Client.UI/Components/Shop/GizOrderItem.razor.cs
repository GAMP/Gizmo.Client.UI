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
        private UserCartProductViewState _previousOrderLine;
        private UserCartProductItemViewState _productItemViewState;

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        public UserCartProductViewState OrderLine { get; set; }


        public string GetPurchaseOptionsGroup()
        {
            return "PurchaseOptions_" + OrderLine.ProductId;
        }

        public Task OnRemoveQuantityButtonClickHandler(MouseEventArgs _) => 
            UserCartService.RemoveUserCartProductAsync(OrderLine.ProductId);

        public Task OnAddQuantityButtonClickHandlerAsync(MouseEventArgs _) => 
            UserCartService.AddUserCartProductAsync(OrderLine.ProductId);

        public Task SetPayType(bool isChecked, OrderLinePayType payType)
        {
            if (isChecked)
                return UserCartService.ChangeProductPayType(OrderLine.ProductId, payType);
            else
                return Task.CompletedTask;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);

            var orderLineChanged = _previousOrderLine != OrderLine;

            if (orderLineChanged)
            {
                _previousOrderLine = OrderLine;

                if (_productItemViewState != null)
                {
                    //The same component used again with a different order line.
                    //We have to unbind from the old product.
                    this.UnsubscribeChange(_productItemViewState);
                }

                //We have to bind to the new product.
                _productItemViewState = await UserCartService.GetCartProductItemViewStateAsync(OrderLine.ProductId);
                this.SubscribeChange(_productItemViewState);
            }
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_productItemViewState);
            base.Dispose();
        }
    }
}
