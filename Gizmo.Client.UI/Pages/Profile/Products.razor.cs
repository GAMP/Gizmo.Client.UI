using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserProductsRoute)]
    public partial class Products : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        TimeProductsViewState ViewState { get; set; }

        [Inject]
        ProductDetailsPageViewState ProductDetailsPageViewState { get; set; }

        /// <summary>
        /// Gets whether purchased products can be navigated to their details page.
        /// Disabled when the shop is off or product details are disabled.
        /// </summary>
        private bool ProductDetailsNavigationEnabled => ProductDetailsPageViewState.ProductDetailsNavigationEnabled;

        public void OpenDetails(int productId)
        {
            if (!ProductDetailsNavigationEnabled)
                return;

            NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={productId}");
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(ProductDetailsPageViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(ProductDetailsPageViewState);

            base.Dispose();
        }
    }
}
