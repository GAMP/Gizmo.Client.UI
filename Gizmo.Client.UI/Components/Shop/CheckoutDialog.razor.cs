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
        private bool _isOpen { get; set; }

        private IEnumerable<PaymentMethodViewState> _paymentMethods = Enumerable.Empty<PaymentMethodViewState>();

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartViewService UserCartService { get; set; }

        [Inject]
        UserCartViewState ViewState { get; set; }

        [Inject]
        PaymentMethodViewStateLookupService PaymentMethodViewStateLookupService { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

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

            var tmp = await PaymentMethodViewStateLookupService.GetStatesAsync();

            _paymentMethods = tmp.Where(a => a.Id != -4 && !a.IsOnline && !a.IsDeleted && a.IsEnabled).ToList();

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
