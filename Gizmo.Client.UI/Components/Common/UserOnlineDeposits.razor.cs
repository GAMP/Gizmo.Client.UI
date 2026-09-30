using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class UserOnlineDeposits : ShellComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserOnlineDepositViewService UserOnlineDepositViewStateService { get; set; }

        [Inject]
        UserOnlineDepositViewState ViewState { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickPayFromPC { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickClear { get; set; }

        [Parameter]
        public EventCallback OnPaymentSucceeded { get; set; }

        #region PAYMENT RESULT

        private static readonly TimeSpan SuccessAnimationDuration = TimeSpan.FromSeconds(5);

        private bool _paymentSucceeded;
        private bool _paymentFailed;
        private decimal? _succeededAmount;
        private CancellationTokenSource _successDelayCts;

        protected string SucceededAmountFormatted =>
            (_succeededAmount ?? 0).ToString("C", CultureInfo.CurrentCulture);

        private void OnDepositStateChanged(object sender, EventArgs e)
        {
            if (ViewState.PageIndex == 0)
            {
                _paymentSucceeded = false;
                _paymentFailed = false;
                return;
            }

            if (!ClientFeatures.PaymentEvents || _paymentSucceeded)
                return;

            if (PaymentIntentStatus.IsPaid(ViewState))
            {
                _succeededAmount = ViewState.Amount;
                _paymentSucceeded = true;
                _paymentFailed = false;

                DispatchWorkflow(RunSuccessSequenceAsync);
            }
            else if (PaymentIntentStatus.IsFailed(ViewState) != _paymentFailed)
            {
                _paymentFailed = !_paymentFailed;

                DispatchRender();
            }
        }

        private async Task RunSuccessSequenceAsync()
        {
            StateHasChanged();

            _successDelayCts = new CancellationTokenSource();
            try
            {
                await Task.Delay(SuccessAnimationDuration, _successDelayCts.Token);
                await OnPaymentSucceeded.InvokeAsync();
            }
            catch (TaskCanceledException)
            {
            }
        }

        #endregion

        private Task OnClickPayFromPCHandler(MouseEventArgs args)
        {
            return OnClickPayFromPC.InvokeAsync(args);
        }

        private Task OnClickClearHandler(MouseEventArgs args)
        {
            return OnClickClear.InvokeAsync(args);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            ViewState.OnChange += OnDepositStateChanged;

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            ViewState.OnChange -= OnDepositStateChanged;
            _successDelayCts?.Cancel();

            base.Dispose();
        }
    }
}
