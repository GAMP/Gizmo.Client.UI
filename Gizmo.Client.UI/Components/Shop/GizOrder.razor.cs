using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrder : CustomDOMComponentBase
    {
        [Inject]
        UserCartService UserCartService { get; set; }

        public bool PaymentMethodSelectorIsOpen { get; set; }

        private Task PlaceOrder()
        {
            PaymentMethodSelectorIsOpen = true;

            StateHasChanged();

            return Task.CompletedTask;
        }

        public void SelectPaymentMethod(int id)
        {
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserCartService.ViewState);
            base.OnInitialized();
        }
    }
}
