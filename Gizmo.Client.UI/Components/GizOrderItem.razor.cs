using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrderItem
    {
        public GizOrderItem()
        {
            OrderLine = new OrderLineViewModel();

            OrderLine.ProductName = "Monster Energy Ultra Ripper 500ml";
            OrderLine.Quantity = 1;
            OrderLine.UnitPrice = 3.1m;
        }

        [Parameter]
        public OrderLineViewModel OrderLine { get; set; }

        public async Task RemoveProduct()
        {
            await OnRemoveProduct.InvokeAsync(OrderLine.ProductId);
        }

        public void RemoveQuantity()
        {
            if (OrderLine.Quantity > 1)
                OrderLine.Quantity -= 1;
        }

        public void AddQuantity()
        {
            OrderLine.Quantity += 1;
        }

        [Parameter]
        public EventCallback<int> OnRemoveProduct { get; set; }

    }
}