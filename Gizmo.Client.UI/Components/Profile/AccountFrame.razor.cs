using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Components
{
    public partial class AccountFrame : ShellComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        protected string DisplayName => UserViewState.IsGuest || string.IsNullOrWhiteSpace(Profile.Username)
            ? GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_GUEST))
            : Profile.Username;

        protected string FullName => string.Join(" ", new[] { Profile.FirstName, Profile.LastName }.Where(part => !string.IsNullOrWhiteSpace(part)));

        protected bool ShowMemberSince => !UserViewState.IsGuest && Profile.RegistrationDate != default;

        protected string SinceText => ShowMemberSince ? MemberSince : string.Empty;

        protected string NickClass => DisplayName.Length switch
        {
            > 16 => "giz-jersey__nick giz-jersey__nick--long",
            > 10 => "giz-jersey__nick giz-jersey__nick--mid",
            _ => "giz-jersey__nick",
        };

        protected bool HasEmail => !string.IsNullOrWhiteSpace(Profile.Email);

        protected bool HasPhone => !string.IsNullOrWhiteSpace(Profile.MobilePhone);

        protected bool CanTopUp => OnlineDeposit.IsEnabled;

        protected string CurrentPackageName => TimeProducts.IsInitialized == true
            ? TimeProducts.TimeProducts
                .Where(a => a.ActivationOrder == 1)
                .Select(a => TimeProductText.Name(a, GrafitLocalization))
                .FirstOrDefault()
            : null;

        private Task ChangePassword() => ChangePasswordService.StartAsync(true, true);

        private async Task TopUp()
        {
            var dialog = await DialogService.ShowUserOnlineDepositsDialogAsync();

            if (dialog.Result == AddComponentResultCode.Opened)
                _ = await dialog.WaitForResultAsync();
        }

        protected string MemberSince => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_ACCOUNT_MEMBER_SINCE, Profile.RegistrationDate.ToLocalTime().ToString("d MMMM yyyy", CultureInfo.CurrentCulture));

        protected string TimeText => Balance.Time.HasValue && TimeLeft.NotNegative(Balance.Time) is var time
            ? $"{(int)time.TotalHours}{TimeUnit(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_HOUR_ABBREVIATED))} {time.Minutes:00}{TimeUnit(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PRODUCT_TIME_EXPIRATION_MINUTE_ABBREVIATED))}"
            : string.Empty;

        private string TimeUnit(string key) => GrafitLocalization.GetString(key)?.TrimEnd('.', ' ') ?? string.Empty;

        protected bool HasTimeCredit => Credit.TimeCreditType != CreditType.NoCredit && Credit.IsUserTimeCreditEnabled;

        protected bool HasSalesCredit => Credit.SalesCreditType != CreditType.NoCredit;

        protected bool HasCredit => HasTimeCredit || HasSalesCredit;

        protected bool TimeCreditUnlimited => HasTimeCredit && Credit.TimeCreditType == CreditType.Unlimited;

        protected bool SalesCreditUnlimited => HasSalesCredit && Credit.SalesCreditType == CreditType.Unlimited;

        protected bool CreditUnlimited => TimeCreditUnlimited || SalesCreditUnlimited;

        protected bool CreditIsMixed => HasTimeCredit && HasSalesCredit && TimeCreditUnlimited != SalesCreditUnlimited;

        protected string BalanceText => Balance.Balance.ToString("C", CultureInfo.CurrentCulture);

        protected string PointsText => Balance.PointsBalance.ToString("N0", CultureInfo.CurrentCulture);

        protected string CreditLimitText => Credit.CreditLimit.ToString("C", CultureInfo.CurrentCulture);

        protected string CreditNote => CreditUnlimited
            ? GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PROFILE_CREDIT_TOOLTIP_UNLIMITED_CREDIT_DESCRIPTION))
            : GrafitLocalization.GetString(HasTimeCredit ? nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PROFILE_CREDIT_TOOLTIP_TIME_CREDIT_DESCRIPTION) : nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_PROFILE_CREDIT_TOOLTIP_SALES_CREDIT_DESCRIPTION));

        [Inject]
        UserProfileViewState Profile { get; set; }

        [Inject]
        UserViewState UserViewState { get; set; }

        [Inject]
        UserBalanceViewState Balance { get; set; }

        [Inject]
        CreditOptionsViewState Credit { get; set; }

        [Inject]
        IOptionsMonitor<ClientInterfaceOptions> InterfaceOptions { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        UserChangePasswordViewService ChangePasswordService { get; set; }

        [Inject]
        UserOnlineDepositViewState OnlineDeposit { get; set; }

        [Inject]
        TimeProductsViewState TimeProducts { get; set; }

        [Inject]
        IClientDialogService DialogService { get; set; }

        private static readonly string[] PROGRESS_ROUTES =
        {
            ClientRoutes.UserLadderRoute,
            ClientRoutes.UserAchievementsRoute,
            ClientRoutes.UserChallengesRoute,
        };

        private static readonly string[] TIME_ROUTES =
        {
            ClientRoutes.UserProductsRoute,
            ClientRoutes.UserProfileRoute,
        };

        private string CurrentPath => "/" + NavigationManager.ToBaseRelativePath(NavigationManager.Uri).Split('?', '#')[0].TrimEnd('/');

        private bool IsOn(string[] routes) => routes.Any(route => string.Equals(route.TrimEnd('/'), CurrentPath, System.StringComparison.OrdinalIgnoreCase));

        private string ProgressTabClass => IsOn(PROGRESS_ROUTES) ? "giz-account__tab active" : "giz-account__tab";

        private string TimeTabClass => IsOn(TIME_ROUTES) ? "giz-account__tab active" : "giz-account__tab";

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(Profile);
            this.SubscribeChange(UserViewState);
            this.SubscribeChange(Balance);
            this.SubscribeChange(Credit);
            this.SubscribeChange(OnlineDeposit);
            this.SubscribeChange(TimeProducts);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Profile);
            this.UnsubscribeChange(UserViewState);
            this.UnsubscribeChange(Balance);
            this.UnsubscribeChange(Credit);
            this.UnsubscribeChange(OnlineDeposit);
            this.UnsubscribeChange(TimeProducts);

            base.Dispose();
        }
    }
}
