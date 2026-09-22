using System.Threading.Tasks;

using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductQuantityPicker : ShellComponentBase
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

        [Inject]
        IClientDialogService DialogService { get; set; }

        public bool IsShopEnabled => !ShopOptions.CurrentValue.Disabled;

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public bool IsFullWidth { get; set; } = true;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// Time package: bought outright rather than added to the cart.
        /// </summary>
        private bool IsTimePackage => _product?.ProductType == ProductType.ProductTime;

        //While the purchase dialog is open the button goes dark, or a second press sends a
        //second copy of the package to the cart.
        private bool _buying;

        public async Task OnAddProductButtonClickHandler(MouseEventArgs args)
        {
            await OnClick.InvokeAsync(args);
            ClientServerCartViewService.AddProduct(ProductId);
        }

        /// <summary>
        /// Buy on a time package: add to the cart and open the purchase dialog.
        /// </summary>
        /// <remarks>
        /// The click is deliberately NOT bubbled through <see cref="OnClick"/>: on a product
        /// card that opens the product page, and the customer has already said what they
        /// want.
        /// </remarks>
        public Task OnBuyPackageClickHandler(MouseEventArgs args)
        {
            if (_buying)
                return Task.CompletedTask;

            _buying = true;

            DispatchWorkflow(async () =>
            {
                try
                {
                    await PackagePurchaseFlow.RunAsync(ProductId, ClientServerCartViewService, DialogService);
                }
                finally
                {
                    _buying = false;
                    StateHasChanged();
                }
            });

            return Task.CompletedTask;
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
        //ShellComponentBase.DispatchWorkflow).
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
