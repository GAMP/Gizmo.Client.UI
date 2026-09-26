using System;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.LoginRoute)]
    public partial class Login : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        private FieldIdentifier? _countryFieldIdentifier;

        // Calling-code digits of the last selected country, used to re-seed the login name
        // when the user switches back to phone mode (mirrors the pre-refactor behavior).
        private string? _selectedCallingCodeDigits;

        [Inject]
        IOptions<UserLoginOptions> UserLoginOptions { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        UserLoginViewState ViewState { get; set; }

        [Inject]
        HostQRCodeViewState HostQRCodeViewState { get; set; }

        [Inject()]
        HostReservationViewState HostReservationViewState { get; set; }

        [Inject()]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; init; }

        [Inject()]
        UserLoginConfigurationViewState UserLoginConfigurationViewState { get; init; }

        [Inject]
        HostLockViewService HostUserLockService { get; set; }

        [Inject]
        IOptions<HostQRCodeOptions> HostQrCodeOptions { get; set; }

        protected string QrTitle => string.IsNullOrEmpty(HostQrCodeOptions.Value.Title)
            ? LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_LOGIN_QR_TITLE))
            : HostQrCodeOptions.Value.Title;

        protected string QrMessage => string.IsNullOrEmpty(HostQrCodeOptions.Value.Description)
            ? LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_LOGIN_QR_MESSAGE))
            : HostQrCodeOptions.Value.Description;

        private FieldIdentifier GetCountryFieldIdentifier()
        {
            _countryFieldIdentifier ??= new FieldIdentifier(ViewState, nameof(ViewState.Country));
            return _countryFieldIdentifier.Value;
        }

        private Task OnCountryChangedAsync(PhoneCountrySelection? selection)
        {
            if (selection == null)
            {
                _selectedCallingCodeDigits = null;
                UserLoginService.SetCountry(null);
                UserLoginService.SetRegionCode(null);
                UserLoginService.SetLoginName(string.Empty);
            }
            else
            {
                _selectedCallingCodeDigits = selection.CallingCodeDigits;
                UserLoginService.SetCountry(selection.CountryName);
                UserLoginService.SetRegionCode(selection.RegionCode);
                if (ViewState.LoginType == View.UserLoginType.MobilePhone)
                    UserLoginService.SetLoginName(selection.CallingCodeDigits);
            }
            return Task.CompletedTask;
        }

        private Task OnPhoneValueChangedAsync(string? value)
        {
            UserLoginService.SetLoginName(value ?? string.Empty);
            return Task.CompletedTask;
        }

        private Task OnKeyDownHandle(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                return UserLoginService.LoginAsync();
            return Task.CompletedTask;
        }

        internal static string StripLeadingCountryDigit(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value[0] == '7' || value[0] == '8' ? value.Substring(1) : value;
        }

        private void SetPhoneLoginName(string value)
        {
            UserLoginService.SetLoginName(StripLeadingCountryDigit(value));
        }

        private void SelectLoginType(ICollection<Button> selectedItems)
        {
            if (selectedItems.Any(a => a.Name == "Username"))
            {
                UserLoginService.SetLoginMethod(View.UserLoginType.UsernameOrEmail);
            }
            else
            {
                var switching = ViewState.LoginType != View.UserLoginType.MobilePhone;
                UserLoginService.SetLoginMethod(View.UserLoginType.MobilePhone);

                // Re-seed the calling code when switching into phone mode with a country already
                // selected (the digits are captured from the last country selection in this session).
                if (switching && !string.IsNullOrEmpty(ViewState.Country) && !string.IsNullOrEmpty(_selectedCallingCodeDigits))
                    UserLoginService.SetLoginName(_selectedCallingCodeDigits);
            }
        }

        public void OnCloseButtonClickHandler()
        {
            UserLoginService.Reset();
        }

        private bool ShowPasswordRecovery =>
            UserRegisterConfigurationViewState.IsPasswordRecoveryEnabled;

        private string? ReservationMessage
        {
            get
            {
                if (HostReservationViewState.ReservationBlockTimeReached)
                    return LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_HOST_RESERVATION_LOGIN_BLOCK_TIME_REACHED_MESSAGE));

                if (HostReservationViewState.ReservationNotificationTimeReached)
                    return LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_HOST_RESERVATION_LOGIN_NOTIFICATION_TIME_REACHED_MESSAGE));

                return null;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(HostQRCodeViewState);
            this.SubscribeChange(HostReservationViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(HostReservationViewState);
            this.UnsubscribeChange(HostQRCodeViewState);
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
