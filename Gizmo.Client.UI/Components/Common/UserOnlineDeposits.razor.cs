using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;
using System.Threading;
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

namespace Gizmo.Client.UI.Components
{
    public partial class UserOnlineDeposits : ShellComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserOnlineDepositViewService UserOnlineDepositViewStateService { get; set; }

        [Inject]
        UserOnlineDepositViewState ViewState { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickPayFromPC { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClickClear { get; set; }

        [Parameter]
        public EventCallback OnPaymentSucceeded { get; set; }

        [Parameter]
        public RenderFragment Head { get; set; }

        [Parameter]
        public bool FocusAmount { get; set; }

        protected sealed record RailStop(decimal Value, string Label, string ClassName, string At, string Pressed);

        private TextInput<decimal?> _amountInput;
        private bool _amountFocused;

        #region PAYMENT RESULT

        private static readonly TimeSpan SuccessAnimationDuration = TimeSpan.FromSeconds(5);

        private bool _paymentSucceeded;
        private bool _paymentFailed;
        private decimal? _succeededAmount;
        private CancellationTokenSource _successDelayCts;

        protected string ClassName => _paymentSucceeded
            ? "giz-user-online-deposit giz-user-online-deposit--done"
            : IsWaiting
                ? "giz-user-online-deposit giz-user-online-deposit--waiting"
                : "giz-user-online-deposit";

        private bool IsWaiting => ViewState.PageIndex != 0 && !_paymentFailed;

        protected string AmountText => Money(ViewState.Amount ?? 0);

        protected bool IsSubmitDisabled => ViewState.IsValid == false || ViewState.IsLoading || (ViewState.Amount ?? 0) <= 0;

        protected string PayClass => ViewState.IsLoading
            ? "giz-buy__btn giz-buy__btn--primary giz-buy__btn--busy giz-user-online-deposit__submit"
            : "giz-buy__btn giz-buy__btn--primary giz-user-online-deposit__submit";

        protected string PayLabel => (ViewState.Amount ?? 0) > 0
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_TOPUP_PAY_AMOUNT, AmountText)
            : GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_ONLINE_DEPOSIT_TITLE));

        protected string AmountNote
        {
            get
            {
                if (ViewState.MinimumAmount is decimal min && ViewState.MaximumAmount is decimal max)
                    return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_TOPUP_LIMITS, Money(min), Money(max));

                if (ViewState.MinimumAmount is decimal from)
                    return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_TOPUP_MIN, Money(from));

                if (ViewState.MaximumAmount is decimal to)
                    return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_TOPUP_MAX, Money(to));

                return string.Empty;
            }
        }

        protected string NoteClass => ViewState.Amount.HasValue && ViewState.IsValid == false
            ? "giz-user-online-deposit__note giz-user-online-deposit__note--invalid"
            : "giz-user-online-deposit__note";

        protected static string CurrencySymbol => CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;

        protected static bool CurrencyFirst => CultureInfo.CurrentCulture.NumberFormat.CurrencyPositivePattern is 0 or 2;

        protected bool HasPresets => ViewState.Presets.Any();

        private List<decimal> Presets => ViewState.Presets.Distinct().OrderBy(a => a).ToList();

        protected IReadOnlyList<RailStop> Stops
        {
            get
            {
                var presets = Presets;

                return presets
                    .Select((value, index) => new RailStop(
                        value,
                        Number(value),
                        StopClass(value),
                        Percent(presets.Count > 1 ? (double)index / (presets.Count - 1) : 0.5),
                        value == ViewState.Amount ? "true" : "false"))
                    .ToList();
            }
        }

        protected string FillText => Percent(Fill);

        private double Fill
        {
            get
            {
                var presets = Presets;

                if (presets.Count < 2 || ViewState.Amount is not decimal amount || amount <= presets[0])
                    return 0;

                if (amount >= presets[^1])
                    return 1;

                var below = presets.FindLastIndex(a => a <= amount);
                var part = (double)((amount - presets[below]) / (presets[below + 1] - presets[below]));

                return (below + part) / (presets.Count - 1);
            }
        }

        private string StopClass(decimal value)
        {
            if (value == ViewState.Amount)
                return "giz-user-online-deposit__stop giz-user-online-deposit__stop--on";

            return value < (ViewState.Amount ?? 0)
                ? "giz-user-online-deposit__stop giz-user-online-deposit__stop--past"
                : "giz-user-online-deposit__stop";
        }

        protected bool StepDownDisabled => !Presets.Any(a => a < (ViewState.Amount ?? 0));

        protected bool StepUpDisabled => ViewState.Amount is decimal amount && !Presets.Any(a => a > amount);

        protected void StepDown()
        {
            var lower = Presets.Where(a => a < (ViewState.Amount ?? 0)).ToList();

            if (lower.Count > 0)
                UserOnlineDepositViewStateService.SelectPreset(lower[^1]);
        }

        protected void StepUp()
        {
            var higher = Presets.Where(a => ViewState.Amount is not decimal amount || a > amount).ToList();

            if (higher.Count > 0)
                UserOnlineDepositViewStateService.SelectPreset(higher[0]);
        }

        protected string LessLabel => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_TOPUP_LESS);

        protected string MoreLabel => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_TOPUP_MORE);

        protected static string Money(decimal value) => value.ToString("C", CultureInfo.CurrentCulture);

        private static string MoneyShort(decimal value) => value.ToString(value % 1 == 0 ? "C0" : "C", CultureInfo.CurrentCulture);

        private static string Number(decimal value) => value.ToString(value % 1 == 0 ? "N0" : "N2", CultureInfo.CurrentCulture);

        private static string Percent(double fraction) => (fraction * 100).ToString("0.###", CultureInfo.InvariantCulture) + "%";

        protected string SucceededSum =>
            CultureInfo.CurrentCulture.NumberFormat.PositiveSign + MoneyShort(_succeededAmount ?? 0);

        private void OnDepositStateChanged(object sender, EventArgs e)
        {
            if (ViewState.PageIndex != 0)
                _amountFocused = false;

            if (ViewState.PageIndex == 0)
            {
                _paymentSucceeded = false;
                _paymentFailed = false;
                return;
            }

            if (!ClientFeatures.PaymentEvents || _paymentSucceeded)
                return;

            if (PaymentIntentStatus.IsPaid(ViewState))
            {
                _succeededAmount = ViewState.Amount;
                _paymentSucceeded = true;
                _paymentFailed = false;

                DispatchWorkflow(RunSuccessSequenceAsync);
            }
            else if (PaymentIntentStatus.IsFailed(ViewState) != _paymentFailed)
            {
                _paymentFailed = !_paymentFailed;

                DispatchRender();
            }
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

        private async Task FinishNowAsync()
        {
            _successDelayCts?.Cancel();
            await OnPaymentSucceeded.InvokeAsync();
        }

        private Task RetryAsync() => OnClickClear.InvokeAsync(new MouseEventArgs());

        private Task OnClickPayFromPCHandler(MouseEventArgs args)
        {
            return OnClickPayFromPC.InvokeAsync(args);
        }

        private Task OnClickClearHandler(MouseEventArgs args)
        {
            return OnClickClear.InvokeAsync(args);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!FocusAmount || _amountFocused || _amountInput is null || ViewState.PageIndex != 0 || _paymentSucceeded || _paymentFailed)
                return;

            _amountFocused = true;
            await _amountInput.FocusAsync();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            ViewState.OnChange += OnDepositStateChanged;

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            ViewState.OnChange -= OnDepositStateChanged;
            _successDelayCts?.Cancel();

            base.Dispose();
        }
    }
}
