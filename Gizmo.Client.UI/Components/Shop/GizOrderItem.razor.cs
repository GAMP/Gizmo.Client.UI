using Gizmo.Client.UI.Pages;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrderItem : CustomDOMComponentBase
    {

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        public UserCartProductViewState OrderLine { get; set; }

        UserCartProductItemViewState ProductItemViewState { get; set; }

        public string GetPurchaseOptionsGroup()
        {
            return "PurchaseOptions_" + OrderLine.ProductId;
        }

        public Task OnRemoveQuantityButtonClickHandler(MouseEventArgs _) => 
            UserCartService.RemoveProductAsync(OrderLine.ProductId, 1);

        public Task OnAddQuantityButtonClickHandlerAsync(MouseEventArgs _) => 
            UserCartService.AddProductAsync(OrderLine.ProductId, 1);

        public Task SetPayType(bool isChecked, OrderLinePayType payType)
        {
            if (isChecked)
                return UserCartService.ChangeProductPayType(OrderLine.ProductId, payType);
            else
                return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            ProductItemViewState = await UserCartService.GetCartProductItemViewStateAsync(OrderLine.ProductId);

            this.SubscribeChange(ProductItemViewState);
            base.OnInitialized();
        }
        public override void Dispose()
        {
            this.UnsubscribeChange(ProductItemViewState);
            base.Dispose();
        }
    }
}
