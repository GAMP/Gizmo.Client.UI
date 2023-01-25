using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
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
        public int ProductId { get; set; }

        public Task AddProduct(int id)
        {
            return UserCartService.AddProductAsyc(id);
        }

        protected override async Task OnInitializedAsync()
        {
            await ProductDetailsPageService.LoadProductAsync(ProductId);

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