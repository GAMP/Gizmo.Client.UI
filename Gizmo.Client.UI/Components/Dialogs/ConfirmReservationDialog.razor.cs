using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ConfirmReservationDialog : CustomDOMComponentBase
    {
        private IEnumerable<PaymentMethodViewState> _paymentMethods = Enumerable.Empty<PaymentMethodViewState>();

        [Inject]
        PaymentMethodViewStateLookupService PaymentMethodViewStateLookupService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ConfirmReservationDialogViewService ConfirmReservationDialogViewService { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<EmptyComponentResult> ResultCallback { get; set; }

        private async Task CloseDialog()
        {
            await ResultCallback.InvokeAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ConfirmReservationDialogViewService.ViewState);

            var tmp = await PaymentMethodViewStateLookupService.GetStatesAsync();

            _paymentMethods = tmp.Where(a => a.Id != -4 && !a.IsOnline && !a.IsDeleted && a.IsEnabled).ToList(); //TODO: AAAAA

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ConfirmReservationDialogViewService.ViewState);

            base.Dispose();
        }
    }
}
