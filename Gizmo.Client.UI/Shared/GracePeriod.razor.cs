using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Shared
{
    public partial class GracePeriod : ShellComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ILogger<GracePeriod> Logger { get; set; }

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

        [Inject]
        ActiveApplicationsViewState ActiveApplicationsViewState { get; set; }

        private const int RUNNING_APPS_SHOWN = 3;

        private ElementReference _pinElement;
        private bool _focusPin;

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

        private bool ShowKept => !UserViewState.IsGuest;

        private string KeptTimeText => UserBalanceViewState.Time is TimeSpan time && time > TimeSpan.Zero
            ? string.Create(CultureInfo.CurrentCulture, $"{(int)time.TotalHours}:{time.Minutes:00}")
            : null;

        private string KeptBalanceText => UserBalanceViewState.Balance.ToString("C", CultureInfo.CurrentCulture);

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

        private IReadOnlyList<AppExeViewState> RunningApps => ActiveApplicationsViewState.Executables
            .Take(RUNNING_APPS_SHOWN)
            .ToList();

        private string PinLabelText => string.IsNullOrEmpty(_pinError)
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_GRACE_PIN_LABEL)
            : _pinError;

        private string PinLabelClass => string.IsNullOrEmpty(_pinError)
            ? "giz-handover__label"
            : "giz-handover__label giz-handover__label--error";

        private double GraceLeft
        {
            get
            {
                return _graceTotal <= TimeSpan.Zero
                    ? 1d
                    : Math.Clamp(ViewState.Time.TotalSeconds / _graceTotal.TotalSeconds, 0d, 1d);
            }
        }

        private string RingStyle => $"--giz-grace-left: {GraceLeft.ToString("0.###", CultureInfo.InvariantCulture)}";

        private string ProgressStyle
        {
            get
            {
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
                    _focusPin = true;
                }
                else if (state.Step == 1)
                {
                }
                else
                {
                    _reservationConfirmed = true;
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Reservation PIN confirmation failed.");
                _pinError = GrafitLocalization.GetString(GrafitResourceKeys.SHELL_GRACE_PIN_FAILED);
                _focusPin = true;
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

        private void TrackGraceTotal()
        {
            if (!ViewState.IsInGracePeriod)
                _graceTotal = TimeSpan.Zero;
            else if (ViewState.Time > _graceTotal)
                _graceTotal = ViewState.Time;
        }

        protected override bool ShouldRender()
        {
            TrackGraceTotal();

            return base.ShouldRender();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_focusPin && !_pinBusy)
            {
                _focusPin = false;

                await ElementFocus.TryAsync(() => _pinElement.FocusAsync());
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            TrackGraceTotal();
            this.SubscribeChange(ViewState);
            this.SubscribeChange(HostReservationViewState);
            this.SubscribeChange(ConfirmReservationDialogViewService.ViewState);
            this.SubscribeChange(UserBalanceViewState);
            this.SubscribeChange(ActiveApplicationsViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(HostReservationViewState);
            this.UnsubscribeChange(ConfirmReservationDialogViewService.ViewState);
            this.UnsubscribeChange(UserBalanceViewState);
            this.UnsubscribeChange(ActiveApplicationsViewState);

            base.Dispose();
        }
    }
}
