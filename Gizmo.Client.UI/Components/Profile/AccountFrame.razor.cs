using System.Globalization;
using System.Linq;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

using ShellText = Gizmo.Client.UI.Localization.ShellStringOverrides;

namespace Gizmo.Client.UI.Components
{
    public partial class AccountFrame : ShellComponentBase
    {
        protected string DisplayName => UserViewState.IsGuest || string.IsNullOrWhiteSpace(Profile.Username)
            ? ShellText.Get(ShellText.GEN_GUEST)
            : Profile.Username;

        protected string FullName => string.Join(" ", new[] { Profile.FirstName, Profile.LastName }.Where(part => !string.IsNullOrWhiteSpace(part)));

        protected bool ShowMemberSince => !UserViewState.IsGuest && Profile.RegistrationDate != default;

        protected string MemberSince => ShellText.Get(ShellText.ACCOUNT_MEMBER_SINCE, Profile.RegistrationDate.ToLocalTime().ToString("d MMMM yyyy", CultureInfo.CurrentCulture));

        protected string TimeText => Balance.Time is { } time ? $"{(int)time.TotalHours}:{time:mm}" : string.Empty;

        protected bool HasTimeCredit => Credit.TimeCreditType != CreditType.NoCredit && Credit.IsUserTimeCreditEnabled;

        protected bool HasSalesCredit => Credit.SalesCreditType != CreditType.NoCredit;

        protected bool HasCredit => HasTimeCredit || HasSalesCredit;

        protected bool CreditUnlimited => Credit.TimeCreditType == CreditType.Unlimited || Credit.SalesCreditType == CreditType.Unlimited;

        protected string CreditLimitText => Credit.CreditLimit.ToString("C", CultureInfo.CurrentCulture);

        protected string CreditNote => CreditUnlimited
            ? ShellText.Get(ShellText.CREDIT_UNLIMITED)
            : ShellText.Get(HasTimeCredit ? ShellText.CREDIT_TIME_NOTE : ShellText.CREDIT_SALES_NOTE);

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
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(Profile);
            this.SubscribeChange(UserViewState);
            this.SubscribeChange(Balance);
            this.SubscribeChange(Credit);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Profile);
            this.UnsubscribeChange(UserViewState);
            this.UnsubscribeChange(Balance);
            this.UnsubscribeChange(Credit);

            base.Dispose();
        }
    }
}
