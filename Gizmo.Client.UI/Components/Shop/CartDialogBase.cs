using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
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
        Balance,
        Points,
        Counter,
    }

    public sealed record PayWay(PayWayKind Kind, int? MethodId, string Name);

    public sealed record PaySegment(PayWayKind Kind, PayWay Way, string Name, string Note, bool NoteIsPoints, bool IsShort, bool IsOn);

    public abstract class CartDialogBase : ShellComponentBase
    {
        #region CONSTANTS

        protected const int DEPOSIT_PAYMENT_METHOD_ID = -3;

        protected const int POINTS_PAYMENT_METHOD_ID = -4;

        protected static readonly TimeSpan CART_SETTLE_CEILING = TimeSpan.FromSeconds(15);

        protected enum Step { Confirm, TopUp, Done }

        #endregion

        #region FIELDS

        protected readonly List<PayWay> _ways = new();

        protected Step _step = Step.Confirm;
        protected bool _paying;

        protected bool? _outcomeOk;
        protected string _outcomeError = string.Empty;
        protected PayWayKind _paidWith;

        #endregion

        #region PROPERTIES

        [Inject] protected ILogger<CartDialogBase> Logger { get; set; }
        [Inject] protected UserCartViewService CartOrderService { get; set; }
        [Inject] protected ClientServerCartViewService CartService { get; set; }
        [Inject] protected PaymentMethodViewStateLookupService PaymentMethodLookupService { get; set; }
        [Inject] protected UserBalanceViewState UserBalanceViewState { get; set; }
        [Inject] protected UserOnlineDepositViewState OnlineDepositViewState { get; set; }
        [Inject] protected UserOnlineDepositViewService OnlineDepositService { get; set; }
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter] public DialogDisplayOptions DisplayOptions { get; set; }
        [Parameter] public EventCallback DismissCallback { get; set; }

        #endregion

        #region STATE OF THE CART

        protected abstract bool IsPriced { get; }

        protected bool IsBusy =>
            CartService.ViewState.IsStateUpdating ||
            CartService.ViewState.IsStateUpdateRequired;

        protected bool IsSettled => IsPriced && !IsBusy;

        protected decimal Total => CartService.ViewState.Total;

        protected decimal SubTotal => CartService.ViewState.SubTotal;

        protected decimal Discount => CartService.ViewState.Discount;

        protected bool HasDiscount => IsPriced && Discount > 0;

        protected decimal Fees
        {
            get
            {
                var extra = Total - (SubTotal - Discount);
                return extra > 0 ? extra : 0;
            }
        }

        protected virtual int PointsDue => CartService.ViewState.PointsTotal;

        protected bool IsFree => IsPriced && Total == 0 && PointsDue == 0;

        #endregion

        #region WAYS TO PAY

        protected virtual bool OffersPoints => false;

        protected virtual bool IsPayingWithPoints => false;

        protected bool IsPayingFromBalance =>
            CartOrderService.ViewState.PaymentMethodId == DEPOSIT_PAYMENT_METHOD_ID;

        protected bool IsPayingAtCounter =>
            !IsPayingWithPoints && SelectedWay?.Kind == PayWayKind.Counter;

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

        protected bool CounterIsGrouped => CounterWays.Count() > 1;

        protected bool ShowChips => CounterIsGrouped && SelectedWay?.Kind == PayWayKind.Counter;

        protected virtual bool WayIsShort(PayWay way) => way.Kind switch
        {
            PayWayKind.Balance => IsPriced && Total > Balance,
            PayWayKind.Points => IsPriced && PointsDue > PointsBalance,
            _ => false,
        };

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
                            cells.Add(new PaySegment(way.Kind, way, way.Name,
                                GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_ON_ACCOUNT, Money(Balance)), false, WayIsShort(way), IsSelected(way)));
                            break;

                        case PayWayKind.Points:
                            cells.Add(new PaySegment(way.Kind, way, way.Name,
                                GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_YOU_HAVE, Points(PointsBalance)), true, WayIsShort(way), IsSelected(way)));
                            break;

                        case PayWayKind.Counter when !CounterIsGrouped:
                            cells.Add(new PaySegment(way.Kind, way, way.Name,
                                GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_AT_COUNTER_NOTE), false, false, IsSelected(way)));
                            break;

                        case PayWayKind.Counter when cells.All(a => a.Kind != PayWayKind.Counter):
                            var chosen = SelectedWay?.Kind == PayWayKind.Counter ? SelectedWay : null;
                            cells.Add(new PaySegment(way.Kind, null,
                                GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_AT_COUNTER),
                                chosen?.Name ?? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_AT_COUNTER_NOTE),
                                false, false, chosen is not null));
                            break;
                    }
                }

                return cells;
            }
        }

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

        protected virtual void Select(PayWay way)
        {
            if (way is null || IsSelected(way) || _paying || !IsPriced)
                return;

            if (CartOrderService.ViewState.PaymentMethodId != way.MethodId)
                CartOrderService.SetOrderPaymentMethod(way.MethodId);

            StateHasChanged();
        }

        protected string SingleWayNote(PayWay way) => way.Kind switch
        {
            PayWayKind.Balance => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_ON_ACCOUNT, Money(Balance)),
            PayWayKind.Points => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_YOU_HAVE, PointsWithUnit(PointsBalance)),
            _ => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_WAY_COUNTER_HINT),
        };

        protected string WaysCaption => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_PAY_WITH);

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
                _ways.Add(new PayWay(PayWayKind.Balance, deposit.Id, GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_WAY_BALANCE)));

            if (OffersPoints)
                _ways.Add(new PayWay(PayWayKind.Points, null, GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_WAY_POINTS)));

            foreach (var method in methods.Where(a => a.Id != DEPOSIT_PAYMENT_METHOD_ID))
                _ways.Add(new PayWay(PayWayKind.Counter, method.Id, method.Name));
        }

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

        protected virtual void SelectPoints() { }

        #endregion

        #region BALANCES

        protected decimal Balance => UserBalanceViewState.Balance;

        protected int PointsBalance => UserBalanceViewState.PointsBalance;

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

        protected decimal BalanceAfter => Balance - Total;

        protected int PointsAfter => PointsBalance - PointsDue;

        protected bool CanTopUp => OnlineDepositViewState.IsEnabled;

        #endregion

        #region PAYING

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

        protected bool NoWayToPay => IsPriced && !IsFree && _ways.Count == 0;

        protected bool PrimaryIsTopUp => IsPriced && !IsFree && Shortfall > 0;

        protected bool PrimaryEnabled => PrimaryIsTopUp ? (CanTopUp && IsSettled && !_paying) : CanPay;

        protected abstract string PayForKey { get; }

        protected string PrimaryText
        {
            get
            {
                if (IsFree)
                    return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_GET_FREE);

                if (PrimaryIsTopUp)
                    return CanTopUp
                        ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_TOP_UP_BY, Money(Shortfall))
                        : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_BUY_TOP_UP_AT_COUNTER);

                if (Total == 0 && PointsDue > 0)
                    return GrafitLocalization.GetString(PayForKey, PointsWithUnit(PointsDue));

                if (IsPayingAtCounter)
                    return GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_SHOP_PLACE_ORDER));

                return GrafitLocalization.GetString(PayForKey, Money(Total));
            }
        }

        protected void Primary()
        {
            if (PrimaryIsTopUp)
                TopUp();
            else
                Pay();
        }

        protected abstract void Pay();

        protected async Task WaitForCartAsync()
        {
            var deadline = DateTime.UtcNow.Add(CART_SETTLE_CEILING);

            await Task.Delay(100);

            while (IsBusy && DateTime.UtcNow < deadline)
                await Task.Delay(100);
        }

        protected void TopUp()
        {
            if (!CanTopUp || Shortfall == 0)
                return;

            OnlineDepositService.SetAmount(Shortfall);

            _step = Step.TopUp;
            StateHasChanged();
        }

        protected void BackToConfirm()
        {
            OnlineDepositService.Clear();

            _step = Step.Confirm;
            StateHasChanged();
        }

        protected void OnTopUpSucceeded() => BackToConfirm();

        protected async Task PayTopUpFromThisPc()
        {
            if (OnlineDepositViewState.PaymentUrl is { } url)
                await InvokeVoidAsync("open", url);
        }

        protected void ResetTopUp() => OnlineDepositService.Clear();

        protected void LeaveTopUp()
        {
            if (_step == Step.TopUp)
                OnlineDepositService.Clear();
        }

        protected virtual Task CloseDialog()
        {
            if (_paying)
                return Task.CompletedTask;

            LeaveTopUp();

            return DismissCallback.InvokeAsync();
        }

        #endregion

        #region TEXTS

        protected abstract string TitleKey { get; }

        protected string Kicker => GrafitLocalization.GetString(
            _step == Step.TopUp ? GrafitResourceKeys.SHELL_BUY_TITLE_TOPUP : TitleKey);

        protected string Money(decimal amount) => amount.ToString("C", CultureInfo.CurrentCulture);

        protected string Points(int amount) => amount.ToString("N0", CultureInfo.CurrentCulture);

        protected string PointsWithUnit(int amount) =>
            GrafitLocalization.GetPluralString(GrafitResourceKeys.SHELL_BUY_POINTS_COUNT, amount);

        #endregion

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(CartOrderService.ViewState);
            this.SubscribeChange(CartService.ViewState);
            this.SubscribeChange(UserBalanceViewState);
            this.SubscribeChange(OnlineDepositViewState);

            await LoadWaysAsync();

            SelectDefaultWay();

            await base.OnInitializedAsync();
        }

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
