using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class CheckoutDialog : CustomDOMComponentBase
    {
        private bool _isOpen { get; set; }

        private IEnumerable<PaymentMethodViewState> _paymentMethods;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartViewService UserCartService { get; set; }

        [Inject]
        UserCartViewState ViewState { get; set; }

        [Inject]
        PaymentMethodViewStateLookupService PaymentMethodViewStateLookupService { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        private void ValueChangedHandler(int? value)
        {
            UserCartService.SetOrderPaymentMethod(value);
        }

        private async Task CloseDialog()
        {
            await DismissCallback.InvokeAsync();

            if (ViewState.IsComplete) //TODO: AAA DO NOT RESET WITH ERRORS.
                await UserCartService.ResetAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ViewState);

            _paymentMethods = await PaymentMethodViewStateLookupService.GetStatesAsync();

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
