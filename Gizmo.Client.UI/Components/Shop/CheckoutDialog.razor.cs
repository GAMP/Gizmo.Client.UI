using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class CheckoutDialog : CustomDOMComponentBase
    {
        private IEnumerable<PaymentMethodViewState> _paymentMethods = Enumerable.Empty<PaymentMethodViewState>();

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartViewService Service { get; set; }

        [Inject]
        ClientServerCartViewService ClientServerCartViewService { get; set; }

        [Inject]
        PaymentMethodViewStateLookupService PaymentMethodViewStateLookupService { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        private void ValueChangedHandler(int? value)
        {
            Service.SetOrderPaymentMethod(value);
        }

        private async Task CloseDialog()
        {
            await DismissCallback.InvokeAsync();

            if (Service.ViewState.IsComplete) //TODO: AAAAA CHECK
                Service.Clear();
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(Service.ViewState);
            this.SubscribeChange(ClientServerCartViewService.ViewState);

            var tmp = await PaymentMethodViewStateLookupService.GetStatesAsync();

            _paymentMethods = tmp.Where(a => a.Id != -4 && !a.IsOnline && !a.IsDeleted && a.IsEnabled).ToList(); //TODO: AAAAA

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ClientServerCartViewService.ViewState);
            this.UnsubscribeChange(Service.ViewState);

            base.Dispose();
        }
    }
}
