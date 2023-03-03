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
    public partial class PaymentMethodSelectorDialog : CustomDOMComponentBase
    {
        public PaymentMethodSelectorDialog()
        {
        }

        private bool _isOpen { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Inject]
        PaymentMethodsService PaymentMethodsService { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

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

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserCartService.ViewState);
            this.SubscribeChange(PaymentMethodsService.ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserCartService.ViewState);
            this.SubscribeChange(PaymentMethodsService.ViewState);
            base.Dispose();
        }
    }
}
