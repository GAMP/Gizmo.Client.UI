using Gizmo.Client.UI.Localization;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Shared
{
    public partial class GracePeriod : ShellComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        GracePeriodViewState ViewState { get; set; }

        [Inject]
        UserViewService UserService { get; set; }

        [Inject]
        UserOnlineDepositViewState UserOnlineDepositViewState { get; set; }

        [Inject]
        UserOnlineDepositViewService UserOnlineDepositViewStateService { get; set; }

        [Inject]
        HostReservationViewState HostReservationViewState { get; set; }

        [Inject]
        HostReservationViewService HostReservationViewService { get; set; }

        //Code confirmation goes through the stock reservation dialog service rather than a
        //direct API call: validation, response handling and the "confirmed but unpaid"
        //branch already live there. The dialog itself is not open, so its internal
        //Result(...) calls simply do nothing.
        [Inject]
        ConfirmReservationDialogViewService ConfirmReservationDialogViewService { get; set; }

        /// <summary>
        /// Whether the card is showing the embedded top-up form instead of
        /// the countdown. Local UI state - the grace period itself is
        /// unaffected either way.
        /// </summary>
        private bool _showDeposit;

        private string _pin = string.Empty;
        private string _pinError;
        private bool _pinBusy;
        private bool _reservationConfirmed;
        private bool _handedOffToPaymentDialog;

        /// <summary>
        /// The grace period was caused by a reservation rather than by a zero balance.
        /// </summary>
        /// <remarks>
        /// The grace period event carries no reason - <c>GracePeriodChangeEventArgs</c> has
        /// only a flag and a time. But this machine's reservation state is right there, and
        /// if its time has arrived (or the sign-in block time has) there is only one cause.
        /// </remarks>
        private bool IsReservationLock =>
            !_handedOffToPaymentDialog
            && HostReservationViewState.ReservationId.HasValue
            && (HostReservationViewState.ReservationTimeReached || HostReservationViewState.ReservationBlockTimeReached);

        private bool ReservationNeedsPayment => ConfirmReservationDialogViewService.ViewState.Step == 1;

        private string ReservationTimeText => HostReservationViewState.Time.HasValue
            ? HostReservationViewState.Time.Value.ToString("HH:mm")
            : string.Empty;

        private async Task OnClickPayFromPCHandler()
        {
            if (UserOnlineDepositViewState.PaymentUrl is not null)
                await JsRuntime.InvokeVoidAsync("open", UserOnlineDepositViewState.PaymentUrl);
        }

        private void OnClickClearHandler()
        {
            UserOnlineDepositViewStateService.Clear();
        }

        // No separate "close" concept here - this card only exists while
        // ViewState.IsInGracePeriod is true, which the server itself flips
        // once the balance clears. Dropping back to the countdown view is
        // enough: either that flag flips right behind it and the whole
        // overlay goes away, or there's a brief lag and the customer sees
        // the normal grace-period screen again instead of being stuck on a
        // success screen with nothing left to do.
        private void OnPaymentSucceededHandler()
        {
            UserOnlineDepositViewStateService.Clear();
            _showDeposit = false;
        }

        private void OnPinInput(ChangeEventArgs args)
        {
            _pin = args.Value?.ToString() ?? string.Empty;
            _pinError = null;
        }

        private Task OnPinKeyDown(KeyboardEventArgs args)
        {
            return args.Key == "Enter" ? OnConfirmPinAsync() : Task.CompletedTask;
        }

        private async Task OnConfirmPinAsync()
        {
            if (_pinBusy || string.IsNullOrWhiteSpace(_pin))
                return;

            _pinBusy = true;
            _pinError = null;
            DispatchRender();

            try
            {
                //The server generates the code; there is nothing to validate here, so it
                //goes as typed, only trimmed.
                ConfirmReservationDialogViewService.SetPin(_pin.Trim());
                await ConfirmReservationDialogViewService.ConfirmAsync();

                var state = ConfirmReservationDialogViewService.ViewState;

                if (!string.IsNullOrEmpty(state.ErrorMessage))
                {
                    _pinError = ShellStringOverrides.Get(ShellStringOverrides.GRACE_PIN_WRONG);
                    _pin = string.Empty;
                }
                else if (state.Step == 1)
                {
                    //Confirmed, but the reservation is unpaid - the payment step is next.
                }
                else
                {
                    _reservationConfirmed = true;
                }
            }
            catch
            {
                _pinError = ShellStringOverrides.Get(ShellStringOverrides.GRACE_PIN_FAILED);
            }
            finally
            {
                _pinBusy = false;
                DispatchRender();
            }
        }

        //Reservation payment runs through the stock dialog, which renders in DialogHost at
        //z-index 1001 while this overlay sits at 3000 - so the overlay leaves the screen
        //for the duration. Nothing is blocked: a grace period means the machine still works.
        private Task OnOpenReservationPaymentAsync()
        {
            _handedOffToPaymentDialog = true;
            DispatchRender();

            return HostReservationViewService.ShowDialog();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(HostReservationViewState);
            this.SubscribeChange(ConfirmReservationDialogViewService.ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(HostReservationViewState);
            this.UnsubscribeChange(ConfirmReservationDialogViewService.ViewState);

            base.Dispose();
        }
    }
}
