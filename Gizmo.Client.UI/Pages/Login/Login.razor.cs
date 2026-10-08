using System;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.LoginRoute)]
    public partial class Login : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [CascadingParameter] RegistrationCardContext Card { get; set; }

        [CascadingParameter] RecoveryHandoff Recovery { get; set; }

        private FieldIdentifier? _countryFieldIdentifier;

        private string? _selectedCallingCodeDigits;

        private enum LoginStep
        {
            Name,
            Password,
            Qr,
        }

        private LoginStep _step;
        private PasswordInput _passwordInput;
        private TextInput<string> _nameInput;
        private PasswordInput _pinInput;
        private bool _focusPin;
        private string _submittedName;
        private bool _focusPassword;
        private bool _focusName;
        private bool _nameError;
        private bool _passwordError;
        private bool _awaitingResult;
        private bool _phoneTried;

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

        [Inject]
        NavigationService NavigationService { get; set; }

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
            _phoneTried = false;
            UserLoginService.SetLoginName(value ?? string.Empty);
            return Task.CompletedTask;
        }

        private bool IsBusy => ViewState.IsLogginIn || ViewState.IsLogginOut;

        private bool IsUsernameLogin => ViewState.LoginType == View.UserLoginType.UsernameOrEmail;

        private bool IsPhoneLogin => !IsUsernameLogin;

        private bool IsPasswordStep => _step == LoginStep.Password && !string.IsNullOrEmpty(ViewState.LoginName);

        private bool ShowNameError => _nameError && ViewState.HasLoginError;

        private bool ShowPasswordError => _passwordError && ViewState.HasLoginError;

        private string NamePillClass => IsPhoneLogin ? "giz-signin__pill giz-signin__pill--phone" : "giz-signin__pill";

        private bool IsQrStep => HostQRCodeViewState.IsEnabled && (_step == LoginStep.Qr || UserLoginOptions.Value.Disabled);

        private bool IsSignInClosed => UserLoginOptions.Value.Disabled;

        private bool ShowQrLink => HostQRCodeViewState.IsEnabled;

        private bool HasLoginName => !string.IsNullOrWhiteSpace(ViewState.LoginName)
            && (IsUsernameLogin || ViewState.LoginName.Length > (_selectedCallingCodeDigits?.Length ?? 0));

        private bool IsNextDisabled => !HasLoginName;

        private bool HasCheckedPhone => !string.IsNullOrEmpty(ViewState.PhoneE164);

        private string PhoneError => _phoneTried && IsPhoneLogin && !HasCheckedPhone
            ? UserLoginService.EditContext
                .GetValidationMessages(new FieldIdentifier(ViewState, nameof(ViewState.LoginName)))
                .FirstOrDefault()
            : null;

        private bool NeedsReservationPin =>
            HostReservationViewState.ReservationNotificationTimeReached || HostReservationViewState.ReservationBlockTimeReached;

        private string PasswordTitle => IsUsernameLogin
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_LOGIN_HELLO, (ViewState.LoginName ?? string.Empty).Trim())
            : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_LOGIN_PASSWORD_TITLE);

        private string WhoText => IsUsernameLogin
            ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_LOGIN_NOT_YOU)
            : "+" + ViewState.LoginName;

        private void GoToPassword()
        {
            if (!HasLoginName)
                return;

            if (IsPhoneLogin && !HasCheckedPhone)
            {
                _phoneTried = true;
                return;
            }

            _phoneTried = false;
            _step = LoginStep.Password;
            _focusPassword = true;
            _nameError = false;
            _passwordError = false;
        }

        private void BackToName()
        {
            _step = LoginStep.Name;
            _nameError = false;
            _passwordError = false;
            _focusName = true;
            UserLoginService.SetPassword(string.Empty);
        }

        private Task SubmitAsync()
        {
            if (IsBusy)
                return Task.CompletedTask;

            _submittedName = ViewState.LoginName;
            _passwordError = false;
            _awaitingResult = true;

            return UserLoginService.LoginAsync();
        }

        private void OnLoginViewStateChanged(object sender, EventArgs e)
        {
            if (_awaitingResult && !IsBusy && ViewState.HasLoginError && string.IsNullOrEmpty(ViewState.Password))
            {
                _awaitingResult = false;
                _passwordError = true;
            }

            if (_step == LoginStep.Name && _phoneTried && IsPhoneLogin && HasCheckedPhone)
            {
                GoToPassword();
                return;
            }

            if (_step != LoginStep.Password || !string.IsNullOrEmpty(ViewState.LoginName))
                return;

            if (NeedsReservationPin && !string.IsNullOrEmpty(_submittedName))
            {
                UserLoginService.SetLoginName(_submittedName);
                _focusPin = true;
                return;
            }

            _step = LoginStep.Name;
            _nameError = true;
            _focusName = true;
        }

        private void OpenQr() => _step = LoginStep.Qr;

        private void RecoverPassword()
        {
            var name = (ViewState.LoginName ?? string.Empty).Trim();

            if (name.Length > 0)
                Recovery?.Offer(new RecoveryRequest(IsPhoneLogin, name, ViewState.Country, ViewState.RegionCode));

            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryKindRoute);
        }

        private void OnCardChanged(object sender, EventArgs e)
        {
            if (TakeRegisteredName())
                _ = InvokeAsync(StateHasChanged);
        }

        private bool TakeRegisteredName()
        {
            var nick = Card?.TakePendingLoginName();

            if (string.IsNullOrEmpty(nick))
                return false;

            UserLoginService.SetLoginMethod(View.UserLoginType.UsernameOrEmail);
            UserLoginService.SetLoginName(nick);
            _step = LoginStep.Name;
            _focusName = true;
            return true;
        }

        private void OnNameKeyDown(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                GoToPassword();
        }

        private Task OnPasswordKeyDown(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
                return SubmitAsync();

            return Task.CompletedTask;
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

        protected override async Task OnInitializedAsync()
        {
            ViewState.OnChange += OnLoginViewStateChanged;
            this.SubscribeChange(ViewState);
            this.SubscribeChange(HostQRCodeViewState);
            this.SubscribeChange(HostReservationViewState);

            if (Card is not null)
                Card.Changed += OnCardChanged;

            TakeRegisteredName();

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (_focusPin && _pinInput is not null)
            {
                _focusPin = false;
                await ElementFocus.TryAsync(() => _pinInput.FocusAsync());
            }
            else if (_focusPassword && _passwordInput is not null)
            {
                _focusPassword = false;
                await ElementFocus.TryAsync(() => _passwordInput.FocusAsync());
            }
            else if (firstRender || _focusName)
            {
                _focusName = false;

                if (IsUsernameLogin && _nameInput is not null)
                    await ElementFocus.TryAsync(() => _nameInput.FocusAsync());
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override void Dispose()
        {
            if (Card is not null)
                Card.Changed -= OnCardChanged;

            this.UnsubscribeChange(HostReservationViewState);
            this.UnsubscribeChange(HostQRCodeViewState);
            ViewState.OnChange -= OnLoginViewStateChanged;
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
