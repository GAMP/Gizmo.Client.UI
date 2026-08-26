using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    /// <summary>
    /// Buying a time package in one step, without a trip through the shop cart.
    /// </summary>
    /// <remarks>
    /// The cart is still what actually charges the customer - there is exactly
    /// one server side cart per user and no single-product order endpoint on the
    /// client, so "bypassing the cart" means bypassing the cart SCREEN, not the
    /// cart itself. This dialog therefore puts the package in the cart when it
    /// opens, so the total it shows is the real, server computed one (tax, fees
    /// and any promotion included), and takes the package back out again if the
    /// customer closes without paying. Nothing is left behind either way.
    ///
    /// Topping up happens INSIDE this dialog rather than by sending the customer
    /// off to another screen: dialogs here are a queue, not a stack, so a second
    /// dialog would only appear after this one closed, and the purchase would be
    /// lost on the way. The existing deposit component is reused as a second
    /// step with the shortfall already filled in, and when the balance lands the
    /// dialog returns to the confirmation with the money in place.
    /// </remarks>
    public partial class PackagePurchaseDialog : CustomDOMComponentBase
    {
        #region CONSTANTS

        /// <summary>
        /// Gizmo's own well known id for the Deposit (account balance) payment
        /// method - see the switch in PaymentMethodViewStateLookupService.Map.
        /// Not something a club configures, so it is safe to look for directly.
        /// </summary>
        private const int DEPOSIT_PAYMENT_METHOD_ID = -3;

        #endregion

        #region FIELDS

        private IEnumerable<PaymentMethodViewState> _paymentMethods = Enumerable.Empty<PaymentMethodViewState>();
        private UserProductViewState? _product;

        private bool _paid;
        private Step _step = Step.Confirm;

        //Итог покупки, зафиксированный МОИМ окном в момент, когда оплата
        //вернулась. Показывать результат из общего view state нельзя: у
        //UserCartViewService.CheckoutAsync стоит IsComplete = true в блоке
        //finally, то есть ДО того, как исключение дойдёт до внешнего catch и
        //выставит HasError. Любая ошибка, не попавшая в разобранные там коды
        //Cart и Promotion, поэтому сначала показывается как успех, а потом
        //переписывается ошибкой. Сервис живёт в Gizmo.Client.UI.Services,
        //которую сервер из скина не перезагружает, так что править его нельзя -
        //но можно не давать окну переобуваться после показанного результата.
        private bool? _outcomeOk;
        private string _outcomeError = string.Empty;

        private enum Step { Confirm, TopUp }

        #endregion

        #region PROPERTIES

        [Inject] ILocalizationService LocalizationService { get; set; }
        [Inject] UserCartViewService CartOrderService { get; set; }
        [Inject] ClientServerCartViewService CartService { get; set; }
        [Inject] PaymentMethodViewStateLookupService PaymentMethodLookupService { get; set; }
        [Inject] UserProductViewStateLookupService ProductLookupService { get; set; }
        [Inject] UserBalanceViewState UserBalanceViewState { get; set; }
        [Inject] UserOnlineDepositViewState OnlineDepositViewState { get; set; }
        [Inject] UserOnlineDepositViewService OnlineDepositService { get; set; }

        [Parameter] public int ProductId { get; set; }

        /// <summary>
        /// Запись корзины, которую окно показывает и оплачивает.
        /// </summary>
        /// <remarks>
        /// Кладёт пакет в корзину вызывающая сторона, ДО открытия этого окна, и
        /// передаёт сюда готовую запись. Раньше окно добавляло пакет само - и
        /// намертво зависало на пакете с ограниченным временем использования:
        /// проверка в ClientServerCartViewService показывает вопрос «товар сейчас
        /// недоступен, всё равно добавить?» и ждёт ответа, а диалоги здесь -
        /// очередь, так что вопрос стоял за моим окном и на экран не выходил.
        /// </remarks>
        [Parameter] public Guid CartEntryId { get; set; }
        [Parameter] public DialogDisplayOptions DisplayOptions { get; set; }
        [Parameter] public EventCallback DismissCallback { get; set; }

        #endregion

        #region VIEW

        private decimal Total => CartService.ViewState.Total;

        /// <summary>
        /// Цена до скидки и сама скидка, когда она есть.
        /// </summary>
        /// <remarks>
        /// Скидку считает сервер - это может быть промо, тариф группы или ручная
        /// скидка на аккаунте. Показывать её надо: без неё «к оплате 0 ₽» под
        /// пакетом за 650 ₽ выглядит поломкой, а не выгодой.
        /// </remarks>
        private decimal SubTotal => CartService.ViewState.SubTotal;

        private decimal Discount => CartService.ViewState.Discount;

        private bool HasDiscount => IsPriced && Discount > 0;

        private bool IsPayingFromBalance =>
            CartOrderService.ViewState.PaymentMethodId == DEPOSIT_PAYMENT_METHOD_ID;

        /// <summary>
        /// How much the balance is short of the total, or zero.
        /// </summary>
        /// <remarks>
        /// Only meaningful when paying from the balance. Every other method is
        /// settled at the counter, so the account balance says nothing about
        /// whether the order can go through.
        /// </remarks>
        private decimal Shortfall
        {
            get
            {
                if (!IsPayingFromBalance || !IsPriced)
                    return 0;

                var missing = Total - UserBalanceViewState.Balance;
                return missing > 0 ? missing : 0;
            }
        }

        private bool CanTopUp => OnlineDepositViewState.IsEnabled;

        /// <summary>
        /// Whether the cart already held something before this dialog opened.
        /// </summary>
        /// <remarks>
        /// It cannot be hidden: there is one cart, so paying now pays for those
        /// items too. Saying so is the only honest option - quietly charging for
        /// a basket the customer thought they had left for later would be worse
        /// than the extra line of text.
        /// </remarks>
        private bool HasOtherItems => CartService.ViewState.Products.Any(a => a.Guid != CartEntryId);

        private bool IsBusy =>
            CartOrderService.ViewState.IsLoading ||
            CartService.ViewState.IsStateUpdating ||
            CartService.ViewState.IsStateUpdateRequired;

        /// <summary>
        /// Запись пакета лежит в корзине, и сервер уже посчитал по ней сумму.
        /// </summary>
        /// <remarks>
        /// Итог считается на сервере и приходит позже самой записи. Пока его нет,
        /// корзина показывает ноль - а окно с «к оплате 0,00» над активной
        /// кнопкой оплаты это способ случайно нажать её. Ни цена, ни нехватка, ни
        /// кнопка не считаются окончательными, пока запись не на месте.
        /// </remarks>
        private bool IsPriced => CartService.ViewState.Products.Any(a => a.Guid == CartEntryId);

        private bool IsSettled => IsPriced && !IsBusy;

        /// <summary>
        /// Оплата возможна: корзина посчитана, денег хватает и способ оплаты
        /// выбран — либо не нужен вовсе, когда платить нечего.
        /// </summary>
        private bool CanPay =>
            IsSettled &&
            Shortfall == 0 &&
            (Total == 0 || CartOrderService.ViewState.PaymentMethodId.HasValue);

        private bool IsFree => IsPriced && Total == 0;

        private string Duration
        {
            get
            {
                var minutes = _product?.TimeProduct?.Minutes ?? 0;

                if (minutes <= 0)
                    return string.Empty;

                if (minutes < 60)
                    return $"{minutes} мин";

                var hours = minutes / 60;
                var rest = minutes % 60;

                return rest == 0 ? $"{hours} ч" : $"{hours} ч {rest} мин";
            }
        }

        #endregion

        #region FUNCTIONS

        private void OnPaymentMethodChanged(int? value)
        {
            CartOrderService.SetOrderPaymentMethod(value);
        }

        private void Pay()
        {
            DispatchWorkflow(async () =>
            {
                //Нулевой итог (например, стопроцентная скидка) оплачивать
                //нечем: у корзины на нуле не должно быть привязанного способа
                //оплаты. Ровно так же поступает штатный checkout - см.
                //UserCartViewService.SubmitAsync, где при Total == 0
                //ShowPaymentMethods выключается. Привязка депозита с нулевой
                //суммой - самый вероятный источник ошибки, которая приходила
                //уже после показанного «успешно».
                if (Total == 0 && CartOrderService.ViewState.PaymentMethodId.HasValue)
                    CartOrderService.SetOrderPaymentMethod(null);

                await CartOrderService.CheckoutAsync();

                var failed = CartOrderService.ViewState.HasError;

                _outcomeOk = !failed;
                _outcomeError = CartOrderService.ViewState.ErrorMessage;

                //CheckoutAsync сам сбрасывает корзину при успехе, так что
                //вынимать оттуда уже нечего.
                _paid = !failed;
            });
        }

        /// <summary>
        /// Switches to the top-up step with the missing amount already entered.
        /// </summary>
        private void TopUp()
        {
            OnlineDepositService.SetAmount(Shortfall);

            _step = Step.TopUp;
            StateHasChanged();
        }

        private void BackToConfirm()
        {
            _step = Step.Confirm;
            StateHasChanged();
        }

        /// <summary>
        /// The balance landed - back to the confirmation, now payable.
        /// </summary>
        private void OnTopUpSucceeded()
        {
            OnlineDepositService.Clear();
            BackToConfirm();
        }

        private async Task CloseDialog()
        {
            TakeBackPackage();

            await DismissCallback.InvokeAsync();

            CartOrderService.ClearDialog();
        }

        /// <summary>
        /// Убирает из корзины ровно ту запись, ради которой открывалось окно.
        /// Всё, что клиент положил туда сам, остаётся на месте.
        /// </summary>
        private void TakeBackPackage()
        {
            if (_paid || CartEntryId == Guid.Empty)
                return;

            if (CartService.ViewState.Products.Any(a => a.Guid == CartEntryId))
                CartService.RemoveEntry(CartEntryId);
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(CartOrderService.ViewState);
            this.SubscribeChange(CartService.ViewState);
            this.SubscribeChange(UserBalanceViewState);
            this.SubscribeChange(OnlineDepositViewState);

            try
            {
                _product = await ProductLookupService.GetStateAsync(ProductId);
            }
            catch (Exception)
            {
                _product = null;
            }

            try
            {
                var all = await PaymentMethodLookupService.GetStatesAsync();

                //Same filter the shop checkout uses: online methods are handled
                //by their own deposit flow, not by an order payment.
                _paymentMethods = all
                    .Where(a => a.Id != -4 && !a.IsOnline && !a.IsDeleted && a.IsEnabled)
                    .ToList();
            }
            catch (Exception)
            {
                _paymentMethods = Enumerable.Empty<PaymentMethodViewState>();
            }

            //Pay from the balance unless the club does not offer it, so the
            //common case needs no choice at all.
            if (CartOrderService.ViewState.PaymentMethodId is null)
            {
                var preferred = _paymentMethods.FirstOrDefault(a => a.Id == DEPOSIT_PAYMENT_METHOD_ID)
                                ?? _paymentMethods.FirstOrDefault();

                if (preferred is not null)
                    CartOrderService.SetOrderPaymentMethod(preferred.Id);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            //Safety net for a teardown that did not go through CloseDialog.
            TakeBackPackage();

            this.UnsubscribeChange(OnlineDepositViewState);
            this.UnsubscribeChange(UserBalanceViewState);
            this.UnsubscribeChange(CartService.ViewState);
            this.UnsubscribeChange(CartOrderService.ViewState);

            base.Dispose();
        }

        #endregion
    }
}
