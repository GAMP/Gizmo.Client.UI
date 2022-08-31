using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route("/productdetails/{ProductId:int}")]
    public partial class ProductDetails : ComponentBase
    {
        public ProductDetails()
        {
        }

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

            await base.OnInitializedAsync();
        }
    }
}