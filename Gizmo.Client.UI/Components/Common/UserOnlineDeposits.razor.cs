using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
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
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserOnlineDepositViewService UserOnlineDepositViewStateService { get; set; }

        [Inject]
        UserOnlineDepositViewState ViewState { get; set; }

        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickPayFromPC { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickClear { get; set; }

        [Parameter]
        public EventCallback OnPaymentSucceeded { get; set; }

        #region PAYMENT SUCCESS WATCH

        private static readonly TimeSpan SuccessAnimationDuration = TimeSpan.FromSeconds(5);

        private const decimal SuccessDetectionFraction = 0.9m;

        private int _lastPageIndex;
        private bool _watchingBalance;
        private decimal _lastObservedBalance;
        private decimal _positiveBalanceDeltaSinceWatchStart;
        private decimal _watchedAmount;
        private bool _paymentSucceeded;
        private decimal? _succeededAmount;
        private CancellationTokenSource _successDelayCts;

        protected string SucceededAmountFormatted =>
            (_succeededAmount ?? 0).ToString("C", CultureInfo.CurrentCulture);

        private void OnDepositStateChanged(object sender, EventArgs e)
        {
            if (_lastPageIndex != 1 && ViewState.PageIndex == 1)
            {
                StartWatchingBalance();
            }
            else if (ViewState.PageIndex == 0)
            {
                StopWatchingBalance();
                _paymentSucceeded = false;
            }

            _lastPageIndex = ViewState.PageIndex;
        }

        private void StartWatchingBalance()
        {
            if (_watchingBalance)
                return;

            _watchingBalance = true;
            _watchedAmount = ViewState.Amount ?? 0;
            _lastObservedBalance = UserBalanceViewState.Balance;
            _positiveBalanceDeltaSinceWatchStart = 0;
            UserBalanceViewState.OnChange += OnBalanceChanged;
        }

        private void StopWatchingBalance()
        {
            if (!_watchingBalance)
                return;

            _watchingBalance = false;
            UserBalanceViewState.OnChange -= OnBalanceChanged;
        }

        private void OnBalanceChanged(object sender, EventArgs e)
        {
            if (!_watchingBalance || _paymentSucceeded)
                return;

            var current = UserBalanceViewState.Balance;
            var delta = current - _lastObservedBalance;
            _lastObservedBalance = current;

            if (delta > 0)
                _positiveBalanceDeltaSinceWatchStart += delta;

            if (_watchedAmount <= 0 || _positiveBalanceDeltaSinceWatchStart < _watchedAmount * SuccessDetectionFraction)
                return;

            _succeededAmount = _watchedAmount;
            _paymentSucceeded = true;
            StopWatchingBalance();

            DispatchWorkflow(RunSuccessSequenceAsync);
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

            _lastPageIndex = ViewState.PageIndex;
            ViewState.OnChange += OnDepositStateChanged;

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            ViewState.OnChange -= OnDepositStateChanged;
            StopWatchingBalance();
            _successDelayCts?.Cancel();

            base.Dispose();
        }
    }
}
