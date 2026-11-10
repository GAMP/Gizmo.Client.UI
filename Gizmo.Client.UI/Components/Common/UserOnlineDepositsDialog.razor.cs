using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
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

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

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

        protected string Title => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_TOPUP_TITLE);

        protected string CloseLabel => GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_CLOSE));

        private async Task OnDismissHandler()
        {
            await DismissCallback.InvokeAsync();

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
            UserOnlineDepositViewStateService.Clear();

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
