using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class HomePass : ShellComponentBase
    {
        private const int QUEUE_ROWS = 3;

        private static readonly TimeSpan RELOAD_SPACING = TimeSpan.FromMinutes(1);

        private DateTime _loadedAt = DateTime.MinValue;

        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject] UserViewState UserViewState { get; set; }
        [Inject] UserProfileViewState UserProfileViewState { get; set; }
        [Inject] UserBalanceViewState UserBalanceViewState { get; set; }
        [Inject] TimeProductsViewState TimeProductsViewState { get; set; }
        [Inject] TimeProductsViewService TimeProductsViewService { get; set; }
        [Inject] UserOnlineDepositViewState UserOnlineDepositViewState { get; set; }
        [Inject] AssistanceRequestViewState AssistanceRequestViewState { get; set; }
        [Inject] UserMenuViewService UserMenuViewService { get; set; }
        [Inject] LogoViewState LogoViewState { get; set; }
        [Inject] IClientDialogService DialogService { get; set; }

        private bool HasClubLogo => !string.IsNullOrEmpty(LogoViewState.Logo);

        private string Nick => UserViewState.Username ?? string.Empty;

        private IReadOnlyList<string> Bars => PlayerCard.Barcode(Nick);

        private string BalanceText => UserBalanceViewState.Balance.ToString("C", CultureInfo.CurrentCulture);

        private bool HasTimeLimit => UserBalanceViewState.Time.HasValue;

        private string TimeText
        {
            get
            {
                var time = UserBalanceViewState.Time ?? TimeSpan.Zero;
                return string.Create(CultureInfo.CurrentCulture, $"{(int)time.TotalHours}:{time.Minutes:00}");
            }
        }

        private string UntilText => UserBalanceViewState.Time is TimeSpan time && time > TimeSpan.Zero
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_HOME_UNTIL,
                DateTime.Now.Add(time).ToString("t", CultureInfo.CurrentCulture))
            : null;

        private string SinceText => !UserViewState.IsGuest && UserProfileViewState.RegistrationDate != default
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_SIGNUP_SINCE,
                UserProfileViewState.RegistrationDate.ToLocalTime().ToString("yyyy", CultureInfo.CurrentCulture))
            : null;

        private IReadOnlyList<TimeProductViewState> Queue => TimeProductsViewState.IsInitialized == true
            ? TimeProductsViewState.TimeProducts
                .Where(product => product.ActivationOrder.HasValue)
                .OrderBy(product => product.ActivationOrder)
                .Take(QUEUE_ROWS)
                .ToList()
            : Array.Empty<TimeProductViewState>();

        private static string RowClass(TimeProductViewState product) => product.ActivationOrder == 1
            ? "giz-home-pass__row giz-home-pass__row--now"
            : "giz-home-pass__row";

        private bool CanTopUp => UserOnlineDepositViewState.IsEnabled;

        private bool CanCallAdmin => AssistanceRequestViewState.IsEnabled;

        private void CallAdmin() => UserMenuViewService.ToggleAssistanceRequests();

        private async Task TopUp()
        {
            var dialog = await DialogService.ShowUserOnlineDepositsDialogAsync();

            if (dialog.Result == AddComponentResultCode.Opened)
                _ = await dialog.WaitForResultAsync();
        }

        private async Task LoadQueueAsync()
        {
            if (DateTime.UtcNow - _loadedAt < RELOAD_SPACING || (_loadedAt != DateTime.MinValue && !ShellActivity.IsActive))
                return;

            _loadedAt = DateTime.UtcNow;

            try
            {
                await TimeProductsViewService.LoadAsync();
            }
            catch (Exception)
            {
                _loadedAt = DateTime.MinValue;
            }
        }

        private void OnBalanceChanged(object sender, EventArgs e) => DispatchWorkflow(LoadQueueAsync);

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(UserViewState);
            this.SubscribeChange(UserBalanceViewState);
            this.SubscribeChange(TimeProductsViewState);
            this.SubscribeChange(UserOnlineDepositViewState);
            this.SubscribeChange(AssistanceRequestViewState);
            this.SubscribeChange(LogoViewState);

            UserBalanceViewState.OnChange += OnBalanceChanged;

            await LoadQueueAsync();

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            UserBalanceViewState.OnChange -= OnBalanceChanged;

            this.UnsubscribeChange(LogoViewState);
            this.UnsubscribeChange(AssistanceRequestViewState);
            this.UnsubscribeChange(UserOnlineDepositViewState);
            this.UnsubscribeChange(TimeProductsViewState);
            this.UnsubscribeChange(UserBalanceViewState);
            this.UnsubscribeChange(UserViewState);

            base.Dispose();
        }
    }
}
