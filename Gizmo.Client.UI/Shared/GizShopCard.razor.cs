using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class GizShopCard
    {
        [Parameter]
        public ProductViewModel Product { get; set; }

        [Parameter]
        public EventCallback<int> OnAddProduct { get; set; }

        public async Task AddProduct()
        {
            await OnAddProduct.InvokeAsync(Product.Id);
        }
    }
}