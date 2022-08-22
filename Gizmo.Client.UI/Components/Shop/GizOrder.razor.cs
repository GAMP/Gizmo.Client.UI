using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrder : CustomDOMComponentBase
    {
        [Inject]
        UserCartService UserCartService { get; set; }

        #region METHODS

        public void RemoveProduct(int id)
        {
            //TODO: A
            //var existingOrderLine = Order.OrderLines.Where(a => a.ProductId == id).FirstOrDefault();
            //if (existingOrderLine != null)
            //{
            //    Order.OrderLines.Remove(existingOrderLine);
            //}
        }

        #endregion

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserCartService.ViewState);
            base.OnInitialized();
        }
    }
}
