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

        [Inject]
        ConfirmReservationDialogViewService ConfirmReservationDialogViewService { get; set; }

        private bool _showDeposit;

        private string _pin = string.Empty;
        private string _pinError;
        private bool _pinBusy;
        private bool _reservationConfirmed;
        private bool _handedOffToPaymentDialog;

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
                    _pinError = ShellStringOverrides.Get(ShellStringOverrides.GRACE_PIN_WRONG);
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
                _pinError = ShellStringOverrides.Get(ShellStringOverrides.GRACE_PIN_FAILED);
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
