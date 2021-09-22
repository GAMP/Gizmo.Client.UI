using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizProductCard : CustomDOMComponentBase
    {
        [Parameter]
        public ProductViewModel Product { get; set; }

        [Parameter]
        public EventCallback<int> OnAddProduct { get; set; }

        [Parameter]
        public EventCallback<int> OnOpenDetails { get; set; }

        public async Task AddProduct()
        {
            await OnAddProduct.InvokeAsync(Product.Id);
        }

        public async Task OpenDetails()
        {
            await OnOpenDetails.InvokeAsync(Product.Id);
        }
    }
}