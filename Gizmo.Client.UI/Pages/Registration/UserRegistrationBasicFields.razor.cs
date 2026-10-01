using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationBasicFieldsRoute)]
    public partial class UserRegistrationBasicFields : CustomDOMComponentBase
    {
        private string? _selectedPhoneCountryName;

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

        [CascadingParameter]
        RegistrationCardContext Card { get; set; }

        public bool HasAdditionalFields =>
            RegistrationSession.RequiredUserInfo?.Country == true ||
            RegistrationSession.RequiredUserInfo?.Address == true ||
            RegistrationSession.RequiredUserInfo?.City == true ||
            RegistrationSession.RequiredUserInfo?.PostCode == true;

        public bool ShowPassword => true;
        public bool ShowFirstName => RegistrationSession.RequiredUserInfo?.FirstName == true;
        public bool ShowLastName => RegistrationSession.RequiredUserInfo?.LastName == true;
        public bool ShowBirthDate => RegistrationSession.RequiredUserInfo?.BirthDate == true;
        public bool ShowSex => RegistrationSession.RequiredUserInfo?.Sex == true;
        public bool ShowEmail => RegistrationSession.Flow != RegistrationFlow.Email && RegistrationSession.RequiredUserInfo?.Email == true;
        public bool ShowMobilePhone => !RegistrationSession.HasConfirmedMobilePhone && RegistrationSession.RequiredUserInfo?.Mobile == true;
        public bool ShowPhone => RegistrationSession.RequiredUserInfo?.Phone == true;

        public string FirstNameLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_FIRST_NAME));
        public string LastNameLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LAST_NAME));
        public string BirthDateLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_BIRTH_DATE));
        public string SexLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_GENDER));
        public string EmailLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_EMAIL_ADDRESS));
        public string MobilePhoneLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_MOBILE_PHONE));
        public string PhoneLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_ADDITIONAL_PHONE));

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

        private const int PhoneMaxLength = 20;

        private Task<bool> ValidatePhoneCharacterAsync(char character)
        {
            var current = ViewState.Phone ?? string.Empty;

            if (current.Length >= PhoneMaxLength)
                return Task.FromResult(false);

            if (char.IsDigit(character))
                return Task.FromResult(true);

            // '+' is only ever allowed into an empty field, so it can only land in the first position.
            if (character == '+')
                return Task.FromResult(current.Length == 0);

            return Task.FromResult(false);
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

        private void OnViewStateChanged(object sender, EventArgs e) =>
            Card?.SetDraft(PlayerCardData.FromBasics(ViewState, RegistrationSession));

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

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            ViewState.OnChange += OnViewStateChanged;
            OnViewStateChanged(this, EventArgs.Empty);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            ViewState.OnChange -= OnViewStateChanged;
            this.UnsubscribeChange(ViewState);
            base.Dispose();
        }
    }
}
