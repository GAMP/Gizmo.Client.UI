using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrderItem : CustomDOMComponentBase
    {
        public GizOrderItem()
        {
        }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        public UserCartProductViewState OrderLine { get; set; }

        public string GetPurchaseOptionsGroup()
        {
            return "PurchaseOptions_" + OrderLine.ProductId;
        }

        public Task RemoveProduct()
        {
            return UserCartService.RemoveProductAsync(OrderLine.ProductId, null);
        }

        public Task RemoveQuantity()
        {
            return UserCartService.RemoveProductAsync(OrderLine.ProductId, 1);
        }

        public Task AddQuantity()
        {
            return UserCartService.AddProductAsyc(OrderLine.ProductId, 1);
        }
    }
}