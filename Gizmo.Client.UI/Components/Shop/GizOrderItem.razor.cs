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
        ClientServerCartViewService ClientServerCartViewService { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }


        [Parameter]
        public UserCartProductViewState UserCartProductViewState { get; set; }

        public string GetPurchaseOptionsGroup()
        {
            return "PurchaseOptions_" + UserCartProductViewState.ProductId;
        }

        public void OnRemoveQuantityButtonClickHandler(MouseEventArgs _) =>
            ClientServerCartViewService.SetQuantity(UserCartProductViewState.Guid, UserCartProductViewState.Quantity - 1);

        public void OnAddQuantityButtonClickHandlerAsync(MouseEventArgs _) =>
            ClientServerCartViewService.SetQuantity(UserCartProductViewState.Guid, UserCartProductViewState.Quantity + 1);

        public void SetPayType(bool isChecked, OrderLinePayType payType)
        {
            if (isChecked)
                ClientServerCartViewService.SetPayType(UserCartProductViewState.Guid, payType);
        }

        protected override async Task OnInitializedAsync()
        {
            if (UserCartProductViewState != null)
            {
                this.SubscribeChange(UserCartProductViewState);

                if (UserCartProductViewState.ProductId.HasValue)
                {
                    _product = await UserProductViewStateLookupService.GetStateAsync(UserCartProductViewState.ProductId.Value);

                    if (_product != null)
                        this.SubscribeChange(_product);
                }
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_product != null)
                this.UnsubscribeChange(_product);

            if (UserCartProductViewState != null)
                this.UnsubscribeChange(UserCartProductViewState);

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
