using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationPhoneRoute)]
    public partial class UserRegistrationPhone : CustomDOMComponentBase
    {
        private bool _isLoaded;
        private readonly Dictionary<string, string> _countryRegionCodes = new();
        private readonly Dictionary<string, string?> _countryMasks = new();

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        IPhoneValidationService PhoneValidationService { get; set; }

        [Inject]
        IServerInfoService ServerInfo { get; set; }

        [Inject]
        UserRegistrationPhoneViewService RegistrationPhoneViewService { get; set; }

        [Inject]
        UserRegistrationPhoneViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public List<IconSelectCountry> Countries { get; set; } = new List<IconSelectCountry>();

        public string GetMask()
        {
            var selectedCountry = GetSelectedCountry();

            if (selectedCountry != null && _countryMasks.TryGetValue(selectedCountry.Text, out var mask) && mask != null)
                return mask;

            return "###-###-####";
        }

        private string? GetRegionCode(string countryName)
        {
            _countryRegionCodes.TryGetValue(countryName, out var rc);
            return rc;
        }

        public void OnCloseButtonClickHandler()
        {
            RegistrationPhoneViewService.Reset();
        }

        public void OnClickClearValueButtonHandler(MouseEventArgs args)
        {
            SetSelectedCountry(null);
        }

        public IconSelectCountry GetSelectedCountry()
        {
            if (string.IsNullOrEmpty(ViewState.Country))
            {
                return null;
            }
            else
            {
                return Countries.Where(a => a.Text == ViewState.Country).FirstOrDefault();
            }
        }

        protected void SetSelectedCountry(IconSelectCountry value)
        {
            if (value == null)
            {
                RegistrationPhoneViewService.SetCountry(null);
                RegistrationPhoneViewService.SetRegionCode(null);
                RegistrationPhoneViewService.SetMobilePhone(null);
            }
            else
            {
                RegistrationPhoneViewService.SetCountry(value.Text);
                RegistrationPhoneViewService.SetRegionCode(GetRegionCode(value.Text));
                var tmp = value.PhonePrefix;
                if (tmp.StartsWith("+"))
                {
                    tmp = tmp.Substring(1);
                }
                RegistrationPhoneViewService.SetMobilePhone(tmp);
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var countries = await PhoneValidationService.GetCountriesAsync();

            foreach (var country in countries.OrderBy(c => c.CountryName))
            {
                _countryRegionCodes[country.CountryName] = country.RegionCode;
                _countryMasks[country.CountryName] = country.InputMask;

                Countries.Add(new IconSelectCountry()
                {
                    Text = country.CountryName,
                    PhonePrefix = country.CallingCode,
                    Icon = country.Flag ?? string.Empty
                });
            }

            foreach (var item in Countries)
            {
                item.Display = item.Text + " " + item.PhonePrefix;
            }

            if (ViewState.Country is null)
            {
                var regionCode = await ServerInfo.GetRegionCodeAsync();
                var def = CountryDefaults.ResolveDefault(regionCode, countries);
                var match = def != null ? Countries.FirstOrDefault(c => c.Text == def.CountryName) : null;
                SetSelectedCountry(match);
            }
            _isLoaded = true;
            await InvokeAsync(StateHasChanged);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        private FieldIdentifier? _countryFieldIdentifier;

        private FieldIdentifier GetCountryFieldIdentifier()
        {
            if (_countryFieldIdentifier == null)
            {
                _countryFieldIdentifier = new FieldIdentifier(ViewState, nameof(ViewState.Country));
            }

            return _countryFieldIdentifier.Value;
        }
    }
}
