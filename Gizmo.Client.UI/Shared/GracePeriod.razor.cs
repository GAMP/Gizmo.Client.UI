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
    public partial class GracePeriod : CustomDOMComponentBase
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

        //Подтверждение кода идёт через штатный сервис диалога брони, а не через
        //свой вызов API: там уже и валидация, и разбор ответа, и та ветка, где
        //после подтверждения остаётся оплата. Диалог при этом не открыт — его
        //внутренние Result(...) вызовы просто ни на что не действуют.
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
        /// Льготный период вызван бронью, а не нулевым балансом.
        /// </summary>
        /// <remarks>
        /// Само событие льготного периода приходит от хоста без причины —
        /// в <c>GracePeriodChangeEventArgs</c> есть только флаг и время. Зато
        /// рядом лежит состояние брони этого компьютера, и если её время уже
        /// наступило (или наступило время блокировки входа), причина ровно одна.
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
            DispatchStateHasChanged();

            try
            {
                //Код генерирует сервер, проверять его здесь нечем и незачем -
                //отдаём как есть, только без окружающих пробелов.
                ConfirmReservationDialogViewService.SetPin(_pin.Trim());
                await ConfirmReservationDialogViewService.ConfirmAsync();

                var state = ConfirmReservationDialogViewService.ViewState;

                if (!string.IsNullOrEmpty(state.ErrorMessage))
                {
                    _pinError = "Код не подошёл. Проверьте его в подтверждении брони.";
                    _pin = string.Empty;
                }
                else if (state.Step == 1)
                {
                    //Подтверждено, но бронь ещё не оплачена - дальше платёжный шаг.
                }
                else
                {
                    _reservationConfirmed = true;
                }
            }
            catch
            {
                _pinError = "Не получилось проверить код. Попробуйте ещё раз.";
            }
            finally
            {
                _pinBusy = false;
                DispatchStateHasChanged();
            }
        }

        //Оплату брони ведёт штатный диалог. Он рисуется в DialogHost на z-index
        //1001, а этот оверлей сидит на 3000 - поверх него диалога не увидеть,
        //поэтому оверлей на время оплаты уходит с экрана целиком. Ничего не
        //блокируется: льготный период на то и льготный, что машина ещё работает.
        private Task OnOpenReservationPaymentAsync()
        {
            _handedOffToPaymentDialog = true;
            DispatchStateHasChanged();

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
