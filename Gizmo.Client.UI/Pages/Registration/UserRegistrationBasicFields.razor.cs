using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationBasicFieldsRoute)]
    public partial class UserRegistrationBasicFields : ShellComponentBase
    {
        private const int PhoneMaxLength = 20;

        private string? _selectedPhoneCountryName;
        private int _index;
        private bool _focus;
        private TextInput<string> _nickInput;
        private PasswordInput _passwordInput;
        private PasswordInput _repeatInput;
        private TextInput<string> _firstNameInput;
        private TextInput<string> _lastNameInput;
        private TextInput<string> _emailInput;
        private TextInput<string> _phoneInput;

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [CascadingParameter]
        RegistrationCardContext Card { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewService UserRegistrationBasicFieldsViewService { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public bool HasAdditionalFields => SignupStages.HasAddress(RegistrationSession);
        public bool ShowFirstName => RegistrationSession.RequiredUserInfo?.FirstName == true;
        public bool ShowLastName => RegistrationSession.RequiredUserInfo?.LastName == true;
        public bool ShowBirthDate => RegistrationSession.RequiredUserInfo?.BirthDate == true;
        public bool ShowSex => RegistrationSession.RequiredUserInfo?.Sex == true;
        public bool ShowEmail => SignupStages.HasEmail(RegistrationSession);
        public bool ShowMobilePhone => SignupStages.HasMobile(RegistrationSession);
        public bool ShowPhone => RegistrationSession.RequiredUserInfo?.Phone == true;

        public string FirstNameLabel => Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_FIRST_NAME));
        public string LastNameLabel => Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LAST_NAME));
        public string BirthDateLabel => Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_BIRTH_DATE));
        public string SexLabel => Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_GENDER));
        public string EmailLabel => Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_EMAIL_ADDRESS));
        public string MobilePhoneLabel => Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_MOBILE_PHONE));
        public string PhoneLabel => Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_ADDITIONAL_PHONE));

        private IReadOnlyList<SignupStage> Groups => SignupStages.Account(RegistrationSession);

        protected SignupStage Current => Groups[Math.Clamp(_index, 0, Groups.Count - 1)];

        protected bool IsLast => _index >= Groups.Count - 1;

        protected bool CanGoBack =>
            _index > 0 || RegistrationSession.Flow != RegistrationFlow.None || !string.IsNullOrEmpty(RegistrationSession.Token);

        protected string Question => GrafitLocalization.GetString(Current switch
        {
            SignupStage.Nick => GrafitResourceKeys.SHELL_SIGNUP_Q_NICK,
            SignupStage.Password => GrafitResourceKeys.SHELL_SIGNUP_Q_PASSWORD,
            SignupStage.Name => GrafitResourceKeys.SHELL_SIGNUP_Q_NAME,
            SignupStage.About => GrafitResourceKeys.SHELL_SIGNUP_Q_ABOUT,
            _ => GrafitResourceKeys.SHELL_SIGNUP_Q_CONTACTS,
        });

        protected string Hint => GrafitLocalization.GetString(Current switch
        {
            SignupStage.Nick => GrafitResourceKeys.SHELL_SIGNUP_NICK_HINT,
            SignupStage.Password => GrafitResourceKeys.SHELL_SIGNUP_PASSWORD_HINT,
            SignupStage.Name => GrafitResourceKeys.SHELL_SIGNUP_NAME_HINT,
            SignupStage.Contacts when ShowMobilePhone => GrafitResourceKeys.SHELL_SIGNUP_CONTACTS_HINT,
            _ => GrafitResourceKeys.SHELL_SIGNUP_ABOUT_HINT,
        });

        protected string GoLabel => !IsLast || HasAdditionalFields
            ? Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_CONTINUE))
            : Vendor(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_SIGN_UP_BUTTON));

        protected string MobilePhoneError => MessageOf(nameof(ViewState.MobilePhone));

        private string Vendor(string key) => LocalizationService.GetString(key);

        private string MessageOf(string field) => UserRegistrationBasicFieldsViewService.EditContext
            .GetValidationMessages(new FieldIdentifier(ViewState, field))
            .FirstOrDefault();

        private IEnumerable<string> FieldsOf(SignupStage stage)
        {
            switch (stage)
            {
                case SignupStage.Nick:
                    yield return nameof(ViewState.Username);
                    break;
                case SignupStage.Password:
                    yield return nameof(ViewState.Password);
                    yield return nameof(ViewState.RepeatPassword);
                    break;
                case SignupStage.Name:
                    if (ShowFirstName) yield return nameof(ViewState.FirstName);
                    if (ShowLastName) yield return nameof(ViewState.LastName);
                    break;
                case SignupStage.About:
                    if (ShowBirthDate) yield return nameof(ViewState.BirthDate);
                    if (ShowSex) yield return nameof(ViewState.Sex);
                    break;
                default:
                    if (ShowMobilePhone) yield return nameof(ViewState.MobilePhone);
                    if (ShowEmail) yield return nameof(ViewState.Email);
                    if (ShowPhone) yield return nameof(ViewState.Phone);
                    break;
            }
        }

        private bool HasErrors(SignupStage stage) => FieldsOf(stage).Any(field => !string.IsNullOrEmpty(MessageOf(field)));

        private void ValidateStage(SignupStage stage)
        {
            switch (stage)
            {
                case SignupStage.Nick:
                    if (string.IsNullOrEmpty(ViewState.Username))
                        UserRegistrationBasicFieldsViewService.SetUsername(string.Empty);
                    break;
                case SignupStage.Password:
                    UserRegistrationBasicFieldsViewService.SetPassword(ViewState.Password ?? string.Empty);
                    UserRegistrationBasicFieldsViewService.SetRepeatPassword(ViewState.RepeatPassword ?? string.Empty);
                    break;
                case SignupStage.Name:
                    if (ShowFirstName) UserRegistrationBasicFieldsViewService.SetFirstName(ViewState.FirstName);
                    if (ShowLastName) UserRegistrationBasicFieldsViewService.SetLastName(ViewState.LastName);
                    break;
                case SignupStage.About:
                    if (ShowBirthDate) UserRegistrationBasicFieldsViewService.SetBirthDate(ViewState.BirthDate);
                    if (ShowSex) UserRegistrationBasicFieldsViewService.SetSex(ViewState.Sex);
                    break;
                default:
                    if (ShowMobilePhone && string.IsNullOrEmpty(ViewState.MobilePhone)) UserRegistrationBasicFieldsViewService.SetMobilePhone(ViewState.MobilePhone);
                    if (ShowEmail && string.IsNullOrEmpty(ViewState.Email)) UserRegistrationBasicFieldsViewService.SetEmail(ViewState.Email);
                    if (ShowPhone) UserRegistrationBasicFieldsViewService.SetPhone(ViewState.Phone);
                    break;
            }
        }

        private void GoTo(int index)
        {
            _index = index;
            _focus = true;
            Card?.SetStage(Current);
        }

        protected async Task NextAsync()
        {
            if (ViewState.IsLoading)
                return;

            var stage = Current;
            ValidateStage(stage);

            if (HasErrors(stage))
                return;

            if (IsLast)
            {
                await SubmitAsync();
                return;
            }

            GoTo(_index + 1);
        }

        private Task BackAsync()
        {
            if (_index > 0)
            {
                GoTo(_index - 1);
                return Task.CompletedTask;
            }

            return NavigateBackAsync();
        }

        private Task OnKeyDownAsync(KeyboardEventArgs args) => args.Key == "Enter" ? NextAsync() : Task.CompletedTask;

        private Task OnPhoneCountryChangedAsync(PhoneCountrySelection? selection)
        {
            _selectedPhoneCountryName = selection?.CountryName;

            if (selection == null)
            {
                UserRegistrationBasicFieldsViewService.SetPhoneRegionCode(null);
                UserRegistrationBasicFieldsViewService.SetMobilePhone(null);
            }
            else
            {
                UserRegistrationBasicFieldsViewService.SetPhoneRegionCode(selection.RegionCode);
                UserRegistrationBasicFieldsViewService.SetMobilePhone(selection.CallingCodeDigits);
            }
            return Task.CompletedTask;
        }

        private Task OnPhoneValueChangedAsync(string? value)
        {
            UserRegistrationBasicFieldsViewService.SetMobilePhone(value);
            return Task.CompletedTask;
        }

        private Task<bool> ValidatePhoneCharacterAsync(char character)
        {
            var current = ViewState.Phone ?? string.Empty;

            if (current.Length >= PhoneMaxLength)
                return Task.FromResult(false);

            if (char.IsDigit(character))
                return Task.FromResult(true);

            return Task.FromResult(character == '+' && current.Length == 0);
        }

        public void OnCloseButtonClickHandler()
        {
            UserRegistrationBasicFieldsViewService.Reset();
        }

        private Task NavigateBackAsync()
        {
            RegistrationSession.Clear();

            var route = RegistrationSession.Flow switch
            {
                RegistrationFlow.Email => ClientRoutes.RegistrationEmailRoute,
                RegistrationFlow.Sms   => ClientRoutes.RegistrationPhoneRoute,
                _                      => ClientRoutes.RegistrationProvidersRoute
            };

            NavigationService.NavigateTo(route);
            return Task.CompletedTask;
        }

        private void OnViewStateChanged(object sender, EventArgs e)
        {
            Card?.SetDraft(PlayerCardData.FromBasics(ViewState, RegistrationSession));

            var groups = Groups;

            for (var i = 0; i < Math.Min(_index, groups.Count); i++)
            {
                if (!HasErrors(groups[i]))
                    continue;

                GoTo(i);
                DispatchRender();
                return;
            }
        }

        private async Task SubmitAsync()
        {
            var snapshot = PlayerCardData.FromBasics(ViewState, RegistrationSession);

            void OnCleared(object sender, EventArgs e) => Card?.Complete(snapshot);

            RegistrationSession.Cleared += OnCleared;

            try
            {
                await UserRegistrationBasicFieldsViewService.SubmitAsync();
            }
            finally
            {
                RegistrationSession.Cleared -= OnCleared;
            }
        }

        private Func<ValueTask> FocusTarget => Current switch
        {
            SignupStage.Nick when _nickInput is not null => () => _nickInput.FocusAsync(),
            SignupStage.Password when _passwordInput is not null => () => _passwordInput.FocusAsync(),
            SignupStage.Name when ShowFirstName && _firstNameInput is not null => () => _firstNameInput.FocusAsync(),
            SignupStage.Name when _lastNameInput is not null => () => _lastNameInput.FocusAsync(),
            SignupStage.Contacts when !ShowMobilePhone && ShowEmail && _emailInput is not null => () => _emailInput.FocusAsync(),
            SignupStage.Contacts when !ShowMobilePhone && !ShowEmail && _phoneInput is not null => () => _phoneInput.FocusAsync(),
            _ => null,
        };

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            ViewState.OnChange += OnViewStateChanged;
            Card?.SetStage(Current);
            OnViewStateChanged(this, EventArgs.Empty);
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender || _focus)
            {
                _focus = false;

                if (FocusTarget is { } focus)
                    await ElementFocus.TryAsync(focus);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override void Dispose()
        {
            ViewState.OnChange -= OnViewStateChanged;
            this.UnsubscribeChange(ViewState);
            base.Dispose();
        }
    }
}
