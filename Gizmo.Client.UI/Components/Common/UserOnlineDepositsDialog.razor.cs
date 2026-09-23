using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class UserOnlineDepositsDialog : CustomDOMComponentBase
    {
        [Inject]
        IJSRuntime JSRuntime { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserOnlineDepositViewState ViewState { get; set; }

        [Inject]
        UserOnlineDepositViewService UserOnlineDepositViewStateService { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<EmptyComponentResult> ResultCallback { get; set; }

        private async Task OnDismissHandler()
        {
            await DismissCallback.InvokeAsync();

            //reset previous selection
            UserOnlineDepositViewStateService.Clear();
        }

        private async Task OnClickPayFromPCHandler()
        {
            if (ViewState.PaymentUrl is not null)
            {
                await JSRuntime.InvokeVoidAsync("open", ViewState.PaymentUrl);

                await ResultCallback.InvokeAsync();
            }
        }

        // Clearing the QR resets back to the amount-selection screen and the
        // dialog stays open (matches the top-bar dropdown's own Clear
        // behaviour) - it used to also invoke ResultCallback here, which
        // closes the whole dialog, so the reset was never actually visible
        // before the dialog vanished.
        private void OnClickClearHandler()
        {
            UserOnlineDepositViewStateService.Clear();
        }

        private async Task OnPaymentSucceededHandler()
        {
            UserOnlineDepositViewStateService.Clear();
            await ResultCallback.InvokeAsync();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
