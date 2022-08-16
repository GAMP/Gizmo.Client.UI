using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrder : CustomDOMComponentBase
    {
        [Parameter]
        public OrderViewModel Order { get; set; }

        #region METHODS

        public void RemoveProduct(int id)
        {
            var existingOrderLine = Order.OrderLines.Where(a => a.ProductId == id).FirstOrDefault();
            if (existingOrderLine != null)
            {
                Order.OrderLines.Remove(existingOrderLine);
            }
        }

        #endregion
    }
}
