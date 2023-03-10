using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI.Services;
using Gizmo.UI.View.States;
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
        UserCartService UserCartService { get; set; }

        [Inject]
        PaymentMethodViewStateLookupService PaymentMethodViewStateLookupService { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private void ValueChangedHandler(int? value)
        {
            UserCartService.SetOrderPaymentMethod(value);
        }

        private async Task CloseDialog()
        {
            await CancelCallback.InvokeAsync();

            if (UserCartService.ViewState.IsComplete)
                await UserCartService.ResetAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(UserCartService.ViewState);

            _paymentMethods = await PaymentMethodViewStateLookupService.GetStatesAsync();

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserCartService.ViewState);
            base.Dispose();
        }
    }
}
