using System.Threading.Tasks;

using Gizmo.Client.Options;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductQuantityPicker : CustomDOMComponentBase
    {
        private UserProductViewState _product;

        private UserCartProductViewState _userCartProductViewState;

        [Inject]
        public UserProductViewState Product
        {
            get { return _product; }
            private set { _product = value; }
        }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Inject]
        ClientServerCartViewService ClientServerCartViewService { get; set; }

        [Inject]
        IOptionsMonitor<ClientShopOptions> ShopOptions { get; set; }

        public bool IsShopEnabled => !ShopOptions.CurrentValue.Disabled;

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public bool IsFullWidth { get; set; } = true;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public async Task OnAddProductButtonClickHandler(MouseEventArgs args)
        {
            await OnClick.InvokeAsync(args);
            ClientServerCartViewService.AddProduct(ProductId);
        }

        public async Task OnRemoveQuantityButtonClickHandler(MouseEventArgs args)
        {
            await OnClick.InvokeAsync(args);
            if (_userCartProductViewState != null)
                ClientServerCartViewService.SetQuantity(_userCartProductViewState.Guid, _userCartProductViewState.Quantity - 1);
        }

        public async Task OnAddQuantityButtonClickHandlerAsync(MouseEventArgs args)
        {
            await OnClick.InvokeAsync(args);
            if (_userCartProductViewState != null)
                ClientServerCartViewService.SetQuantity(_userCartProductViewState.Guid, _userCartProductViewState.Quantity + 1);
        }

        protected override async Task OnInitializedAsync()
        {
            _product = await UserProductViewStateLookupService.GetStateAsync(ProductId);

            if (_product != null)
                this.SubscribeChange(_product);

            ClientServerCartViewService.ViewState.OnChange += ViewState_OnChange;

            _userCartProductViewState = await ClientServerCartViewService.GetUserCartProductViewStateAsync(ProductId);

            if (_userCartProductViewState != null)
                this.SubscribeChange(_userCartProductViewState);

            await base.OnInitializedAsync();
        }

        //Cart changes are raised off the UI thread. Not async void: a dispatcher fault during
        //host teardown would be rethrown on the thread pool and exit the client (see
        //CustomComponentBase.DispatchWorkflow).
        private void ViewState_OnChange(object sender, System.EventArgs e)
        {
            DispatchWorkflow(async () =>
            {
                var tmp = await ClientServerCartViewService.GetUserCartProductViewStateAsync(ProductId);

                if (_userCartProductViewState != tmp)
                {
                    if (_userCartProductViewState != null)
                        this.UnsubscribeChange(_userCartProductViewState);

                    _userCartProductViewState = tmp;

                    if (_userCartProductViewState != null)
                        this.SubscribeChange(_userCartProductViewState);

                    StateHasChanged();
                }
            });
        }

        public override void Dispose()
        {
            if (_userCartProductViewState != null)
                this.UnsubscribeChange(_userCartProductViewState);

            if (_product != null)
                this.UnsubscribeChange(_product);

            base.Dispose();
        }
    }
}
