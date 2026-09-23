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

        // Already-registered, already-live: the top bar's own balance figure
        // is driven by this same view state, updated by a genuine server
        // push (IGizmoClient.UserBalanceChange - see
        // UserBalanceViewService.OnUserBalanceChange). Watching it here is
        // how this component notices a deposit completed without needing a
        // payment-status API of its own - there isn't a member-scoped one to
        // reach for: PaymentIntentsWebApiClient lives under
        // api/v3/paymentintents, the same admin-shaped route prefix as the
        // operator-only user management endpoints, and whether a customer's
        // own token is even allowed to call it can't be checked without a
        // live server.
        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickPayFromPC { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickClear { get; set; }

        /// <summary>
        /// Raised once the success animation has played out. The host
        /// (dropdown / dialog / grace period screen) owns what "done" means
        /// for it - closing a popup, closing a dialog, or just dropping back
        /// to the grace-period countdown - so it decides what to do here
        /// rather than this shared component guessing.
        /// </summary>
        [Parameter]
        public EventCallback OnPaymentSucceeded { get; set; }

        #region PAYMENT SUCCESS WATCH

        // Keep in sync with the countdown bar's CSS animation-duration
        // (giz-deposit-countdown in _user-online-deposit.scss) - the bar is
        // a pure CSS animation for smoothness, this timer is what actually
        // fires the close.
        private static readonly TimeSpan SuccessAnimationDuration = TimeSpan.FromSeconds(5);

        // A deposit rarely lands the instant the QR appears - it can take
        // guests a while to actually pay, and the club keeps billing the
        // running session the whole time (Gizmo periodically debits balance
        // for active time). A plain "has the balance reached
        // balance-at-submit + amount" target is therefore close to
        // guaranteed to never be hit for exactly the people this feature is
        // for: anyone topping up because their balance is draining. Instead
        // this sums only the POSITIVE deltas between consecutive balance
        // updates while watching - ordinary debit ticks contribute nothing
        // (and don't push a cumulative target further out of reach the way
        // subtracting them would), while the deposit's own credit(s) still
        // accumulate toward the submitted amount regardless of how much
        // ground the session billing has eaten in between. A same-tick
        // coincidence (a debit batched into the exact same push as the
        // credit) can shave a little off that one delta, hence the 90%
        // threshold rather than requiring the full amount.
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
                // Covers both clearing the QR and the dropdown/dialog closing
                // and reopening fresh - either way any in-flight watch from
                // a previous QR is stale.
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

        // CRASH FIXED HERE (was live on the real server): UserBalanceChange
        // is a push from the Gizmo client's connection to the server, not a
        // UI event - it can (and evidently does) fire on a background/
        // network thread, not the renderer's own thread. The previous
        // version of this handler was `async void`, did plain field writes
        // synchronously on whatever thread called it, then only wrapped the
        // FIRST following step (StateHasChanged) in InvokeAsync - after
        // that awaited call returned, the rest of the method (the 5s delay,
        // then OnPaymentSucceeded.InvokeAsync() cascading into closing the
        // dropdown/dialog, which touches several components' state) had no
        // guarantee of still running on the renderer's sync context. That
        // is exactly what took the whole shell down: the report was "it
        // crashed at auto-close", i.e. in that unmarshaled tail end.
        //
        // Fix: the raw event handler does nothing but cheap field
        // arithmetic (safe off-thread - no Blazor API touched), then hands
        // off to InvokeAsync with the ENTIRE rest of the workflow as one
        // delegate. Every await inside that delegate resumes on the
        // renderer's captured context, so nothing downstream runs
        // unmarshaled again.
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

            // DispatchWorkflow rather than a bare InvokeAsync: the balance push can land while
            // the WebView is being torn down, and InvokeAsync then throws on this very thread -
            // a network thread with nothing above it to catch, which is fatal to the process.
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
