using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private bool _phoneCountriesLoaded;
        private IconSelectCountry? _selectedPhoneCountry;
        private readonly Dictionary<string, string> _phoneCountryRegionCodes = new();
        private readonly Dictionary<string, string?> _phoneCountryMasks = new();

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        IPhoneValidationService PhoneValidationService { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewService UserRegistrationBasicFieldsViewService { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public List<IconSelectCountry> PhoneCountries { get; set; } = new();

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
        public bool ShowMobilePhone => RegistrationSession.Flow != RegistrationFlow.Sms && RegistrationSession.RequiredUserInfo?.Mobile == true;
        public bool ShowPhone => RegistrationSession.RequiredUserInfo?.Phone == true;

        public string FirstNameLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_FIRST_NAME));
        public string LastNameLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LAST_NAME));
        public string BirthDateLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_BIRTH_DATE));
        public string SexLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_GENDER));
        public string EmailLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_EMAIL_ADDRESS));
        public string MobilePhoneLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_MOBILE_PHONE));
        public string PhoneLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_PHONE));

        public IconSelectCountry? GetSelectedPhoneCountry() => _selectedPhoneCountry;

        protected void SetPhoneCountry(IconSelectCountry? value)
        {
            _selectedPhoneCountry = value;

            if (value == null)
            {
                UserRegistrationBasicFieldsViewService.SetPhoneRegionCode(null);
                UserRegistrationBasicFieldsViewService.SetMobilePhone(null);
                return;
            }

            var regionCode = _phoneCountryRegionCodes.TryGetValue(value.Text, out var rc) ? rc : null;
            UserRegistrationBasicFieldsViewService.SetPhoneRegionCode(regionCode);

            var phonePrefix = value.PhonePrefix;
            if (phonePrefix.StartsWith("+"))
                phonePrefix = phonePrefix.Substring(1);

            UserRegistrationBasicFieldsViewService.SetMobilePhone(phonePrefix);
        }

        public string GetPhoneMask()
        {
            if (_selectedPhoneCountry != null && _phoneCountryMasks.TryGetValue(_selectedPhoneCountry.Text, out var mask) && mask != null)
                return mask;
            return "###-###-####";
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
                _                      => ClientRoutes.RegistrationProvidersRoute // redirect flow (Flow=None, Token non-empty)
            };

            NavigationService.NavigateTo(route);
            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var countries = (await PhoneValidationService.GetCountriesAsync()).ToList();

            foreach (var country in countries.OrderBy(c => c.CountryName))
            {
                _phoneCountryRegionCodes[country.CountryName] = country.RegionCode;
                _phoneCountryMasks[country.CountryName] = country.InputMask;

                PhoneCountries.Add(new IconSelectCountry
                {
                    Text = country.CountryName,
                    PhonePrefix = country.CallingCode,
                    Icon = country.Flag ?? string.Empty
                });
            }

            foreach (var item in PhoneCountries)
                item.Display = item.Text + " " + item.PhonePrefix;

            _phoneCountriesLoaded = true;
            await InvokeAsync(StateHasChanged);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
