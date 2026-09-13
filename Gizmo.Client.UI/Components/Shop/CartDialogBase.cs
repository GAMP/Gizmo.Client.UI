using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public enum PayWayKind
    {
        /// <summary>Money already on the account.</summary>
        Balance,
        /// <summary>Loyalty points; a pay type on a cart line, not a payment method.</summary>
        Points,
        /// <summary>A method settled with the staff: cash, card, or a club's own.</summary>
        Counter,
    }

    /// <summary>
    /// One way an order can be paid for.
    /// </summary>
    public sealed record PayWay(PayWayKind Kind, int? MethodId, string Name);

    /// <summary>
    /// One cell of the pay chooser. Counter methods share a cell once there are several,
    /// so the chooser never has more than three cells; the methods themselves are then
    /// picked from chips under it.
    /// </summary>
    /// <param name="Way">The way this cell selects; null for the shared counter cell.</param>
    /// <param name="Note">What the customer has, under the name.</param>
    /// <param name="NoteIsPoints">The note is a points figure and gets the coin.</param>
    public sealed record PaySegment(PayWayKind Kind, PayWay Way, string Name, string Note, bool NoteIsPoints, bool IsShort, bool IsOn);

    /// <summary>
    /// What the two dialogs that take money have in common: the ways to pay and the
    /// chooser built from them, the balances asked of the server, the shortfall and the
    /// top-up step, the result screen.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There is exactly one server side cart per user and no single-product order
    /// endpoint on the client, so every purchase - a package bought in one step from the
    /// home board, or the shop cart at checkout - is the same cart being accepted. What
    /// differs is what is in it and how it got there, which is what the derived dialogs
    /// add.
    /// </para>
    /// <para>
    /// Everything the customer can do about paying happens INSIDE the dialog: choosing
    /// what to pay with and topping up when the balance is short. Dialogs here are a
    /// queue, not a stack, so a second dialog would only appear after this one closed,
    /// and the purchase would be lost on the way.
    /// </para>
    /// </remarks>
    public abstract class CartDialogBase : CustomDOMComponentBase
    {
        #region CONSTANTS

        /// <summary>
        /// Gizmo's own well known id for the Deposit (account balance) payment method - see
        /// the switch in PaymentMethodViewStateLookupService.Map. Not something a club
        /// configures, so it is safe to look for directly.
        /// </summary>
        protected const int DEPOSIT_PAYMENT_METHOD_ID = -3;

        /// <summary>
        /// Well known id of the Points payment method. Points are not an order level
        /// payment method on the client - they are a pay type on the cart line - so this id
        /// is only ever excluded from the method list, as the shop checkout excludes it.
        /// </summary>
        protected const int POINTS_PAYMENT_METHOD_ID = -4;

        /// <summary>
        /// How long to wait for the cart to settle a request queued just before paying.
        /// </summary>
        protected static readonly TimeSpan CART_SETTLE_CEILING = TimeSpan.FromSeconds(15);

        protected enum Step { Confirm, TopUp, Done }

        #endregion

        #region FIELDS

        protected readonly List<PayWay> _ways = new();

        protected Step _step = Step.Confirm;
        protected bool _paying;

        //Outcome as the dialog saw it when the server replied. Fixed once set: the cart
        //raises several changes while the order settles, and a result screen that could
        //flip between them is worse than none.
        protected bool? _outcomeOk;
        protected string _outcomeError = string.Empty;
        protected PayWayKind _paidWith;

        //Balances asked of the server directly on open - see Balance.
        protected decimal? _serverBalance;
        protected int? _serverPoints;

        #endregion

        #region PROPERTIES

        [Inject] protected ILogger<CartDialogBase> Logger { get; set; }
        [Inject] protected UserCartViewService CartOrderService { get; set; }
        [Inject] protected ClientServerCartViewService CartService { get; set; }
        [Inject] protected PaymentMethodViewStateLookupService PaymentMethodLookupService { get; set; }
        [Inject] protected UserBalanceViewState UserBalanceViewState { get; set; }
        [Inject] protected UserOnlineDepositViewState OnlineDepositViewState { get; set; }
        [Inject] protected UserOnlineDepositViewService OnlineDepositService { get; set; }
        [Inject] protected IGizmoClient GizmoClient { get; set; }

        [Parameter] public DialogDisplayOptions DisplayOptions { get; set; }
        [Parameter] public EventCallback DismissCallback { get; set; }

        #endregion

        #region STATE OF THE CART

        /// <summary>
        /// What the dialog sells is in the cart and the server has priced it.
        /// </summary>
        /// <remarks>
        /// The total is computed server side and arrives after the entries themselves.
        /// Until it does the cart reports zero, and a dialog showing "0.00 to pay" above
        /// a live pay button is a way to press it by accident.
        /// </remarks>
        protected abstract bool IsPriced { get; }

        protected bool IsBusy =>
            CartService.ViewState.IsStateUpdating ||
            CartService.ViewState.IsStateUpdateRequired;

        protected bool IsSettled => IsPriced && !IsBusy;

        /// <summary>Money the server asks for the whole cart.</summary>
        protected decimal Total => CartService.ViewState.Total;

        protected decimal SubTotal => CartService.ViewState.SubTotal;

        /// <summary>
        /// The discount the server applied - a promotion, a group tariff or a manual
        /// discount on the account. It has to be shown: "0 to pay" under a 650 package
        /// otherwise looks like breakage rather than a bargain.
        /// </summary>
        protected decimal Discount => CartService.ViewState.Discount;

        protected bool HasDiscount => IsPriced && Discount > 0;

        /// <summary>
        /// Whatever the server added on top of the lines less the discount - tax or a
        /// fee - so the receipt always adds up to the total it shows.
        /// </summary>
        protected decimal Fees
        {
            get
            {
                var extra = Total - (SubTotal - Discount);
                return extra > 0 ? extra : 0;
            }
        }

        /// <summary>Points the server will take for the whole cart.</summary>
        protected virtual int PointsDue => CartService.ViewState.PointsTotal;

        /// <summary>
        /// Nothing at all to pay: a full discount, or something the club gives away.
        /// </summary>
        protected bool IsFree => IsPriced && Total == 0 && PointsDue == 0;

        #endregion

        #region WAYS TO PAY

        /// <summary>
        /// Points can pay for what the dialog sells outright. Off by default: in the shop
        /// cart the points choice is made on each line, on the cart page.
        /// </summary>
        protected virtual bool OffersPoints => false;

        /// <summary>The thing being bought is priced in points rather than money.</summary>
        protected virtual bool IsPayingWithPoints => false;

        /// <summary>Money in the cart is settled from the account balance.</summary>
        protected bool IsPayingFromBalance =>
            CartOrderService.ViewState.PaymentMethodId == DEPOSIT_PAYMENT_METHOD_ID;

        protected bool IsPayingAtCounter =>
            !IsPayingWithPoints && SelectedWay?.Kind == PayWayKind.Counter;

        /// <summary>
        /// The way the purchase is paid for. Money left in the cart from other goods keeps
        /// whatever order payment method is bound, whichever cell is chosen here.
        /// </summary>
        protected PayWay SelectedWay
        {
            get
            {
                if (IsPayingWithPoints)
                    return _ways.FirstOrDefault(a => a.Kind == PayWayKind.Points);

                var methodId = CartOrderService.ViewState.PaymentMethodId;

                return methodId is null ? null : _ways.FirstOrDefault(a => a.MethodId == methodId);
            }
        }

        protected bool IsSelected(PayWay way) => ReferenceEquals(way, SelectedWay);

        protected IEnumerable<PayWay> CounterWays => _ways.Where(a => a.Kind == PayWayKind.Counter);

        /// <summary>
        /// Several methods settled with the staff share one cell of the chooser and are
        /// picked from chips under it; a single one is its own cell under its own name.
        /// </summary>
        protected bool CounterIsGrouped => CounterWays.Count() > 1;

        protected bool ShowChips => CounterIsGrouped && SelectedWay?.Kind == PayWayKind.Counter;

        protected virtual bool WayIsShort(PayWay way) => way.Kind switch
        {
            PayWayKind.Balance => IsPriced && Total > Balance,
            PayWayKind.Points => IsPriced && PointsDue > PointsBalance,
            _ => false,
        };

        /// <summary>
        /// The cells of the pay chooser, in the order the ways were built: balance, points,
        /// counter. Never more than three.
        /// </summary>
        protected IReadOnlyList<PaySegment> Segments
        {
            get
            {
                var cells = new List<PaySegment>();

                foreach (var way in _ways)
                {
                    switch (way.Kind)
                    {
                        case PayWayKind.Balance:
                            cells.Add(new PaySegment(way.Kind, way, way.Name, Money(Balance), false, WayIsShort(way), IsSelected(way)));
                            break;

                        case PayWayKind.Points:
                            cells.Add(new PaySegment(way.Kind, way, way.Name, Points(PointsBalance), true, WayIsShort(way), IsSelected(way)));
                            break;

                        case PayWayKind.Counter when !CounterIsGrouped:
                            cells.Add(new PaySegment(way.Kind, way, way.Name,
                                ShellStringOverrides.Get(ShellStringOverrides.BUY_AT_COUNTER_NOTE), false, false, IsSelected(way)));
                            break;

                        case PayWayKind.Counter when cells.All(a => a.Kind != PayWayKind.Counter):
                            var chosen = SelectedWay?.Kind == PayWayKind.Counter ? SelectedWay : null;
                            cells.Add(new PaySegment(way.Kind, null,
                                ShellStringOverrides.Get(ShellStringOverrides.BUY_AT_COUNTER),
                                chosen?.Name ?? ShellStringOverrides.Get(ShellStringOverrides.BUY_AT_COUNTER_NOTE),
                                false, false, chosen is not null));
                            break;
                    }
                }

                return cells;
            }
        }

        /// <summary>
        /// A cell was pressed: its own way, or - for the shared counter cell - the counter
        /// method already chosen, else the first one.
        /// </summary>
        protected void SelectSegment(PaySegment segment)
        {
            if (segment.Way is not null)
            {
                Select(segment.Way);
                return;
            }

            if (SelectedWay?.Kind == PayWayKind.Counter)
                return;

            var first = CounterWays.FirstOrDefault();

            if (first is not null)
                Select(first);
        }

        /// <summary>
        /// Pays with <paramref name="way"/>. Money methods are a payment method on the
        /// order; a derived dialog that offers points takes care of the cart line.
        /// </summary>
        protected virtual void Select(PayWay way)
        {
            if (way is null || IsSelected(way) || _paying || !IsPriced)
                return;

            if (CartOrderService.ViewState.PaymentMethodId != way.MethodId)
                CartOrderService.SetOrderPaymentMethod(way.MethodId);

            StateHasChanged();
        }

        /// <summary>
        /// The only way there is, stated rather than chosen: what the customer has for it.
        /// </summary>
        protected string SingleWayNote(PayWay way) => way.Kind switch
        {
            PayWayKind.Balance => ShellStringOverrides.Get(ShellStringOverrides.BUY_ON_ACCOUNT, Money(Balance)),
            PayWayKind.Points => ShellStringOverrides.Get(ShellStringOverrides.BUY_YOU_HAVE, PointsWithUnit(PointsBalance)),
            _ => ShellStringOverrides.Get(ShellStringOverrides.BUY_WAY_COUNTER_HINT),
        };

        protected string WaysCaption => ShellStringOverrides.Get(ShellStringOverrides.BUY_PAY_WITH);

        /// <summary>
        /// Builds the list of ways to pay from what the club offers this customer.
        /// </summary>
        /// <remarks>
        /// The account balance comes first when the club allows it, then points when the
        /// dialog offers them, then every method settled at the counter. The method filter
        /// is the one the shop checkout uses: online methods are handled by their own
        /// deposit flow, not by an order payment.
        /// </remarks>
        protected async Task LoadWaysAsync()
        {
            IEnumerable<PaymentMethodViewState> methods;

            try
            {
                var all = await PaymentMethodLookupService.GetStatesAsync();

                methods = all
                    .Where(a => a.Id != POINTS_PAYMENT_METHOD_ID && !a.IsOnline && !a.IsDeleted && a.IsEnabled)
                    .ToList();
            }
            catch (Exception)
            {
                methods = Enumerable.Empty<PaymentMethodViewState>();
            }

            _ways.Clear();

            var deposit = methods.FirstOrDefault(a => a.Id == DEPOSIT_PAYMENT_METHOD_ID);

            if (deposit is not null)
                _ways.Add(new PayWay(PayWayKind.Balance, deposit.Id, ShellStringOverrides.Get(ShellStringOverrides.BUY_WAY_BALANCE)));

            if (OffersPoints)
                _ways.Add(new PayWay(PayWayKind.Points, null, ShellStringOverrides.Get(ShellStringOverrides.BUY_WAY_POINTS)));

            foreach (var method in methods.Where(a => a.Id != DEPOSIT_PAYMENT_METHOD_ID))
                _ways.Add(new PayWay(PayWayKind.Counter, method.Id, method.Name));
        }

        /// <summary>
        /// Picks the way the dialog opens on, unless the customer already chose one.
        /// </summary>
        /// <remarks>
        /// Money before points: the balance is the general case, and the points cell is a
        /// click away with its figure in plain sight. A method chosen earlier that the club
        /// no longer offers is replaced rather than kept.
        /// </remarks>
        protected void SelectDefaultWay()
        {
            var current = CartOrderService.ViewState.PaymentMethodId;

            if (current.HasValue && _ways.Any(a => a.MethodId == current))
                return;

            var preferred = _ways.FirstOrDefault(a => a.Kind == PayWayKind.Balance)
                            ?? _ways.FirstOrDefault(a => a.Kind == PayWayKind.Counter)
                            ?? _ways.FirstOrDefault();

            if (preferred is null)
            {
                if (current.HasValue)
                    CartOrderService.SetOrderPaymentMethod(null);

                return;
            }

            if (preferred.Kind == PayWayKind.Points)
                SelectPoints();
            else
                CartOrderService.SetOrderPaymentMethod(preferred.MethodId);
        }

        /// <summary>Switches the purchase to points. Only a dialog that offers them has one.</summary>
        protected virtual void SelectPoints() { }

        #endregion

        #region BALANCES

        /// <summary>
        /// How much money is on the account.
        /// </summary>
        /// <remarks>
        /// The larger of two numbers, both from the server.
        /// <para>
        /// <c>UserBalanceViewState</c> is updated in exactly two ways: once at login, and
        /// then on the server's <c>UserBalanceChange</c> event. It has no "re-read" of its
        /// own, so a lost event - the client was reconnecting when the top-up landed -
        /// leaves the old number in place and the dialog truthfully but wrongly reports a
        /// shortfall. That is what a club hit: money added, Buy pressed, and the dialog
        /// asked for the full price again.
        /// </para>
        /// <para>
        /// So the balance is also read straight from the server on open into
        /// <c>_serverBalance</c>, and the larger of the two wins: the snapshot can be stale
        /// if something was bought elsewhere, the event can be missing. Erring high is safe -
        /// the server refuses if the money is not there, and the dialog shows that refusal.
        /// Erring low blocks the purchase outright.
        /// </para>
        /// </remarks>
        protected decimal Balance =>
            _serverBalance is { } fetched && fetched > UserBalanceViewState.Balance
                ? fetched
                : UserBalanceViewState.Balance;

        /// <summary>Points on the account, by the same rule as <see cref="Balance"/>.</summary>
        protected int PointsBalance =>
            _serverPoints is { } fetched && fetched > UserBalanceViewState.PointsBalance
                ? fetched
                : UserBalanceViewState.PointsBalance;

        /// <summary>
        /// How much the balance is short of the money to pay, or zero.
        /// </summary>
        /// <remarks>
        /// Only meaningful when the money is settled from the balance. Every other method
        /// is settled at the counter, so the account balance says nothing about whether
        /// the order can go through.
        /// </remarks>
        protected decimal Shortfall
        {
            get
            {
                if (!IsPayingFromBalance || !IsPriced)
                    return 0;

                var missing = Total - Balance;
                return missing > 0 ? missing : 0;
            }
        }

        /// <summary>
        /// Points missing, or zero. Unlike money, points cannot be topped up - they are
        /// earned - so this only ever disables the purchase.
        /// </summary>
        protected int PointsShortfall
        {
            get
            {
                if (!IsPriced)
                    return 0;

                var missing = PointsDue - PointsBalance;
                return missing > 0 ? missing : 0;
            }
        }

        /// <summary>
        /// Money left on the account once the order is paid for. Shown so the customer
        /// sees where they will stand, not only what they give.
        /// </summary>
        protected decimal BalanceAfter => Balance - Total;

        protected int PointsAfter => PointsBalance - PointsDue;

        protected bool CanTopUp => OnlineDepositViewState.IsEnabled;

        /// <summary>
        /// Asks the server for the balances directly. See <see cref="Balance"/> for why.
        /// Not critical: no answer means working from the state as it is.
        /// </summary>
        protected async Task RefreshBalanceAsync()
        {
            try
            {
                var balance = await GizmoClient.UserBalanceGetAsync();

                _serverBalance = balance.Balance;
                _serverPoints = balance.Points;
            }
            catch (Exception)
            {
                //Keep whatever the event delivered.
            }
        }

        #endregion

        #region PAYING

        /// <summary>
        /// Payment is possible: the cart is priced and settled, the points are there, and
        /// any money in it has a method and, from the balance, the funds.
        /// </summary>
        protected virtual bool CanPay
        {
            get
            {
                if (!IsSettled || _paying)
                    return false;

                if (PointsShortfall > 0)
                    return false;

                if (Total > 0)
                {
                    if (!CartOrderService.ViewState.PaymentMethodId.HasValue)
                        return false;

                    if (Shortfall > 0)
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Nothing the club offers can pay for this. Shown instead of a dead button.
        /// </summary>
        protected bool NoWayToPay => IsPriced && !IsFree && _ways.Count == 0;

        /// <summary>
        /// The main button tops up rather than pays: money from the balance is short.
        /// </summary>
        protected bool PrimaryIsTopUp => IsPriced && !IsFree && Shortfall > 0;

        protected bool PrimaryEnabled => PrimaryIsTopUp ? (CanTopUp && IsSettled && !_paying) : CanPay;

        /// <summary>The verb on the main button, with the amount: "Buy for {0}", "Order for {0}".</summary>
        protected abstract string PayForKey { get; }

        protected string PrimaryText
        {
            get
            {
                if (IsFree)
                    return ShellStringOverrides.Get(ShellStringOverrides.BUY_GET_FREE);

                if (PrimaryIsTopUp)
                    return CanTopUp
                        ? ShellStringOverrides.Get(ShellStringOverrides.BUY_TOP_UP_BY, Money(Shortfall))
                        : ShellStringOverrides.Get(ShellStringOverrides.BUY_TOP_UP_AT_COUNTER);

                if (Total == 0 && PointsDue > 0)
                    return ShellStringOverrides.Get(PayForKey, PointsWithUnit(PointsDue));

                if (IsPayingAtCounter)
                    return ShellStringOverrides.Get(ShellStringOverrides.SHOP_PLACE_ORDER);

                return ShellStringOverrides.Get(PayForKey, Money(Total));
            }
        }

        protected void Primary()
        {
            if (PrimaryIsTopUp)
                TopUp();
            else
                Pay();
        }

        /// <summary>Places the order. The derived dialog owns the path and the outcome.</summary>
        protected abstract void Pay();

        /// <summary>
        /// Waits for the cart to finish a request queued a moment ago.
        /// </summary>
        /// <remarks>
        /// Cart requests are buffered for a quarter of a second and then sent; accepting
        /// the order before that would race the change it depends on. The pending flag is
        /// raised a beat after the request is queued, hence the initial pause.
        /// </remarks>
        protected async Task WaitForCartAsync()
        {
            var deadline = DateTime.UtcNow.Add(CART_SETTLE_CEILING);

            await Task.Delay(100);

            while (IsBusy && DateTime.UtcNow < deadline)
                await Task.Delay(100);
        }

        /// <summary>
        /// Switches to the top-up step with the missing amount already entered.
        /// </summary>
        /// <remarks>
        /// The balance is re-read first: the money may have arrived while the dialog was
        /// open - topped up at the counter, say - and nobody should be sent to pay twice.
        /// </remarks>
        protected void TopUp()
        {
            if (!CanTopUp)
                return;

            DispatchWorkflow(async () =>
            {
                await RefreshBalanceAsync();

                if (Shortfall == 0)
                {
                    //Money already there - stay on confirmation.
                    StateHasChanged();
                    return;
                }

                OnlineDepositService.SetAmount(Shortfall);

                _step = Step.TopUp;
                StateHasChanged();
            });
        }

        protected void BackToConfirm()
        {
            _step = Step.Confirm;
            StateHasChanged();
        }

        /// <summary>
        /// The balance landed - back to the confirmation, now payable.
        /// </summary>
        protected void OnTopUpSucceeded()
        {
            OnlineDepositService.Clear();

            DispatchWorkflow(async () =>
            {
                //Ask for the balance rather than waiting for the server event, or the
                //confirmation returns with the old number and asks for a top-up again.
                await RefreshBalanceAsync();

                BackToConfirm();
            });
        }

        protected virtual Task CloseDialog()
        {
            if (_paying)
                return Task.CompletedTask;

            return DismissCallback.InvokeAsync();
        }

        #endregion

        #region TEXTS

        protected abstract string TitleKey { get; }

        protected string Kicker => ShellStringOverrides.Get(
            _step == Step.TopUp ? ShellStringOverrides.BUY_TITLE_TOPUP : TitleKey);

        protected string Money(decimal amount) => amount.ToString("C", CultureInfo.CurrentCulture);

        protected string Points(int amount) => amount.ToString("N0", CultureInfo.CurrentCulture);

        protected string PointsWithUnit(int amount) =>
            ShellStringOverrides.GetPlural(ShellStringOverrides.BUY_POINTS_COUNT, amount);

        #endregion

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(CartOrderService.ViewState);
            this.SubscribeChange(CartService.ViewState);
            this.SubscribeChange(UserBalanceViewState);
            this.SubscribeChange(OnlineDepositViewState);

            //Balances from the server rather than from state - see Balance.
            await RefreshBalanceAsync();

            await OnLoadingAsync();

            await LoadWaysAsync();

            SelectDefaultWay();

            await base.OnInitializedAsync();
        }

        /// <summary>Loads what the derived dialog needs before the ways are built.</summary>
        protected virtual Task OnLoadingAsync() => Task.CompletedTask;

        public override void Dispose()
        {
            this.UnsubscribeChange(OnlineDepositViewState);
            this.UnsubscribeChange(UserBalanceViewState);
            this.UnsubscribeChange(CartService.ViewState);
            this.UnsubscribeChange(CartOrderService.ViewState);

            base.Dispose();
        }

        #endregion
    }
}
