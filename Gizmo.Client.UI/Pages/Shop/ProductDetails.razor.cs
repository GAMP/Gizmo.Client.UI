using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.ProductDetailsRoute)]
    public partial class ProductDetails : CustomDOMComponentBase
    {
        public ProductDetails()
        {
        }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ProductDetailsPageService ProductDetailsPageService { get; set; }

        [Inject]
        ShopPageService ShopService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int ProductId { get; set; }

        public Task AddProduct(int id)
        {
            return UserCartService.AddProductAsyc(id);
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ProductDetailsPageService.ViewState.Product.CartProduct);
            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ProductDetailsPageService.ViewState.Product.CartProduct);
            base.Dispose();
        }
    }
}