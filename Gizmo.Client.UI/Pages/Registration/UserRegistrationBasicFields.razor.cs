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
        IServerInfoService ServerInfo { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewService UserRegistrationBasicFieldsViewService { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public List<IconSelectCountry> PhoneCountries { get; set; } = new();

        public IconSelectCountry? GetSelectedPhoneCountry() => _selectedPhoneCountry;

        protected void SetPhoneCountry(IconSelectCountry value)
        {
            _selectedPhoneCountry = value;
            var regionCode = value != null && _phoneCountryRegionCodes.TryGetValue(value.Text, out var rc) ? rc : null;
            UserRegistrationBasicFieldsViewService.SetPhoneRegionCode(regionCode);
        }

        public string GetPhoneMask()
        {
            if (_selectedPhoneCountry != null && _phoneCountryMasks.TryGetValue(_selectedPhoneCountry.Text, out var mask) && mask != null)
                return mask;
            return "###-###-####";
        }

        public string GetPhonePrefix()
        {
            return _selectedPhoneCountry?.PhonePrefix ?? "+";
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
            var countries = await PhoneValidationService.GetCountriesAsync();

            foreach (var country in countries)
            {
                _phoneCountryRegionCodes[country.CountryName] = country.RegionCode;
                _phoneCountryMasks[country.CountryName] = country.InputMask;

                PhoneCountries.Add(new IconSelectCountry
                {
                    Text = country.CountryName,
                    PhonePrefix = country.CallingCode,
                    Icon = "_content/Gizmo.Client.UI/img/no-flag-image.svg"
                });
            }

            foreach (var item in PhoneCountries)
                item.Display = item.Text + " " + item.PhonePrefix;

            if (_selectedPhoneCountry is null)
            {
                var regionCode = await ServerInfo.GetRegionCodeAsync();
                var def = CountryDefaults.ResolveDefault(regionCode, countries);
                var match = def != null ? PhoneCountries.FirstOrDefault(c => c.Text == def.CountryName) : null;
                if (match != null)
                    SetPhoneCountry(match);
            }

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
