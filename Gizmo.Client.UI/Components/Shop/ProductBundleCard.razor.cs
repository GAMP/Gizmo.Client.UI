using System.Collections.Generic;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductBundleCard : CustomDOMComponentBase
    {
        private bool _clickHandled = false;

        protected bool _shouldRender;

        [Inject]
        ProductDetailsPageViewState ProductDetailsPageViewState { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        UserCartViewService UserCartService { get; set; }

        [Inject]
        UserCartViewState ViewState { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        public void OpenDetails()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            if (!ProductDetailsPageViewState.DisableProductDetails)
                NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={Product.Id}");
        }

        public async Task AddProduct()
        {
            _clickHandled = true;

            await UserCartService.AddUserCartProductAsync(Product.Id);
        }

        public void Ignore()
        {
            _clickHandled = true;
        }

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<UserProductViewState>(nameof(Product), out var newProduct))
            {
                var productChanged = !EqualityComparer<UserProductViewState>.Default.Equals(Product, newProduct);
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
                //await InvokeVoidAsync("writeLine", $"ReRender {this}");
            }
            else
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        #endregion
    }
}
