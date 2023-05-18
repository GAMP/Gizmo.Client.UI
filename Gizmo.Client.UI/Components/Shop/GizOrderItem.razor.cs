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

        [Inject]
        public UserProductViewState Product
        {
            get { return _product; }
            private set { _product = value; }
        }

        [Inject]
        UserCartViewService UserCartService { get; set; }

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
                return UserCartService.ChangeProductPayTypeAsync(ProductItemViewState.ProductId, payType);
            else
                return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            if (ProductItemViewState != null)
            {
                this.SubscribeChange(ProductItemViewState);
            }

            _product = await UserProductViewStateLookupService.GetStateAsync(ProductItemViewState.ProductId);

            if (_product != null)
            {
                this.SubscribeChange(_product);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_product != null)
            {
                this.UnsubscribeChange(_product);
            }

            if (ProductItemViewState != null)
            {
                this.UnsubscribeChange(ProductItemViewState);
            }

            base.Dispose();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
