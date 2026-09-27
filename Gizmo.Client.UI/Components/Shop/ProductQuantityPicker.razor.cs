using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client.Options;
using Gizmo.Client.UI.Localization.Services;
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
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

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
        UserCartViewService UserCartViewService { get; set; }

        public bool IsShopEnabled => !ShopOptions.CurrentValue.Disabled;

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public bool IsFullWidth { get; set; } = true;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        private bool IsTimePackage => _product?.ProductType == ProductType.ProductTime;

        private bool _buying;
        private readonly CancellationTokenSource _lifetime = new();

        public async Task OnAddProductButtonClickHandler(MouseEventArgs args)
        {
            await OnClick.InvokeAsync(args);
            ClientServerCartViewService.AddProduct(ProductId);
        }

        public Task OnBuyPackageClickHandler(MouseEventArgs args)
        {
            if (_buying)
                return Task.CompletedTask;

            _buying = true;

            DispatchWorkflow(async () =>
            {
                try
                {
                    await PackagePurchaseFlow.RunAsync(ProductId, ClientServerCartViewService, UserCartViewService, _lifetime.Token);
                }
                finally
                {
                    _buying = false;

                    if (!IsDisposed)
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
            _lifetime.Cancel();

            ClientServerCartViewService.ViewState.OnChange -= ViewState_OnChange;

            if (_userCartProductViewState != null)
                this.UnsubscribeChange(_userCartProductViewState);

            if (_product != null)
                this.UnsubscribeChange(_product);

            base.Dispose();
        }
    }
}
