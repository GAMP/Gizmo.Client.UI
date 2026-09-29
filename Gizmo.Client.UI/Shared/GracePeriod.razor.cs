using System;
using System.Globalization;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
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
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

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

        [Inject]
        ConfirmReservationDialogViewService ConfirmReservationDialogViewService { get; set; }

        [Inject]
        UserViewState UserViewState { get; set; }

        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        private bool _showDeposit;

        private string _pin = string.Empty;
        private string _pinError;
        private bool _pinBusy;
        private bool _reservationConfirmed;
        private bool _handedOffToPaymentDialog;
        private TimeSpan _graceTotal;

        private bool IsReservationLock =>
            !_handedOffToPaymentDialog
            && HostReservationViewState.ReservationId.HasValue
            && (HostReservationViewState.ReservationTimeReached || HostReservationViewState.ReservationBlockTimeReached);

        private bool ReservationNeedsPayment => ConfirmReservationDialogViewService.ViewState.Step == 1;

        private string ReservationTimeText => HostReservationViewState.Time.HasValue
            ? HostReservationViewState.Time.Value.ToString("t", System.Globalization.CultureInfo.CurrentCulture)
            : string.Empty;

        private string CountdownText => ViewState.Time.ToString(@"mm\:ss", CultureInfo.InvariantCulture);

        private string Username => UserViewState.Username ?? string.Empty;

        private string HandoverModifier => _reservationConfirmed
            ? "giz-handover--done"
            : ReservationNeedsPayment ? "giz-handover--pay" : null;

        private string HandoverTitle => GrafitLocalization.GetString(_reservationConfirmed
            ? GrafitResourceKeys.SHELL_HANDOVER_DONE_TITLE
            : ReservationNeedsPayment ? GrafitResourceKeys.SHELL_HANDOVER_PAY_TITLE : GrafitResourceKeys.SHELL_RESV_PC_RESERVED);

        private string HandoverHint => GrafitLocalization.GetString(_reservationConfirmed
            ? GrafitResourceKeys.SHELL_HANDOVER_DONE_HINT
            : ReservationNeedsPayment ? GrafitResourceKeys.SHELL_HANDOVER_PAY_HINT : GrafitResourceKeys.SHELL_GRACE_RESERVED_HINT);

        private string HandoverCaption => GrafitLocalization.GetString(_reservationConfirmed
            ? GrafitResourceKeys.SHELL_GRACE_CONFIRMED
            : GrafitResourceKeys.SHELL_HANDOVER_UNTIL);

        private string ReservationIcon => _reservationConfirmed ? "ph-bold ph-check" : "ph-fill ph-calendar-check";

        private string ReservationLabel => GrafitLocalization.GetString(_reservationConfirmed
            ? GrafitResourceKeys.SHELL_HANDOVER_YOURS
            : GrafitResourceKeys.SHELL_HANDOVER_RESERVATION);

        private string PinInputClass => string.IsNullOrEmpty(_pinError)
            ? "giz-handover__input"
            : "giz-handover__input giz-handover__input--error";

        private bool IsConfirmDisabled => _pinBusy || string.IsNullOrWhiteSpace(_pin);

        private string DueText => HostReservationViewState.Outstanding is decimal due && due > 0
            ? due.ToString("C", CultureInfo.CurrentCulture)
            : string.Empty;

        private string TimeKeptText
        {
            get
            {
                if (UserBalanceViewState.Time is not TimeSpan time || time <= TimeSpan.Zero)
                    return string.Empty;

                var duration = time.TotalHours >= 1
                    ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DURATION_HOURS_MINUTES, (int)time.TotalHours, time.Minutes)
                    : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DURATION_MINUTES, Math.Max(1, time.Minutes));

                return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_HANDOVER_TIME_KEPT, duration);
            }
        }

        private string ProgressStyle
        {
            get
            {
                if (ViewState.Time > _graceTotal)
                    _graceTotal = ViewState.Time;

                var share = _reservationConfirmed || _graceTotal <= TimeSpan.Zero
                    ? (_reservationConfirmed ? 1d : 0d)
                    : 1d - ViewState.Time.TotalSeconds / _graceTotal.TotalSeconds;

                return $"--giz-handover-progress: {Math.Clamp(share, 0d, 1d).ToString("0.###", CultureInfo.InvariantCulture)}";
            }
        }

        private async Task OnClickPayFromPCHandler()
        {
            if (UserOnlineDepositViewState.PaymentUrl is not null)
                await JsRuntime.InvokeVoidAsync("open", UserOnlineDepositViewState.PaymentUrl);
        }

        private void OnClickClearHandler()
        {
            UserOnlineDepositViewStateService.Clear();
        }

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
                ConfirmReservationDialogViewService.SetPin(_pin.Trim());
                await ConfirmReservationDialogViewService.ConfirmAsync();

                var state = ConfirmReservationDialogViewService.ViewState;

                if (!string.IsNullOrEmpty(state.ErrorMessage))
                {
                    _pinError = GrafitLocalization.GetString(GrafitResourceKeys.SHELL_GRACE_PIN_WRONG);
                    _pin = string.Empty;
                }
                else if (state.Step == 1)
                {
                }
                else
                {
                    _reservationConfirmed = true;
                }
            }
            catch
            {
                _pinError = GrafitLocalization.GetString(GrafitResourceKeys.SHELL_GRACE_PIN_FAILED);
            }
            finally
            {
                _pinBusy = false;
                DispatchRender();
            }
        }

        private Task OnOpenReservationPaymentAsync()
        {
            _handedOffToPaymentDialog = true;
            DispatchRender();

            return HostReservationViewService.ShowDialog();
        }

        protected override bool ShouldRender()
        {
            if (!ViewState.IsInGracePeriod)
                _graceTotal = TimeSpan.Zero;

            return base.ShouldRender();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(HostReservationViewState);
            this.SubscribeChange(ConfirmReservationDialogViewService.ViewState);
            this.SubscribeChange(UserBalanceViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(HostReservationViewState);
            this.UnsubscribeChange(ConfirmReservationDialogViewService.ViewState);
            this.UnsubscribeChange(UserBalanceViewState);

            base.Dispose();
        }
    }
}
