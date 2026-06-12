using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public partial class PasswordRecovery : CustomDOMComponentBase
    {
        private bool _isLoaded;
        private IReadOnlyList<PhoneCountry> _phoneCountries = Array.Empty<PhoneCountry>();
        private readonly Dictionary<string, string> _countryRegionCodes = new();
        private readonly Dictionary<string, string?> _countryMasks = new();
        private FieldIdentifier? _countryFieldIdentifier;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        IPhoneValidationService PhoneValidationService { get; set; }

        [Inject]
        IServerInfoService ServerInfo { get; set; }

        [Inject]
        PasswordRecoveryViewService PasswordRecoveryViewService { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        PasswordRecoveryViewState ViewState { get; set; }

        [Inject]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; init; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public List<IconSelectCountry> Countries { get; set; } = new List<IconSelectCountry>();

        public string GetMask()
        {
            var selected = GetSelectedCountry();
            if (selected != null && _countryMasks.TryGetValue(selected.Text, out var mask) && mask != null)
                return mask;
            return "###-###-####";
        }

        public IconSelectCountry GetSelectedCountry()
        {
            if (string.IsNullOrEmpty(ViewState.Country))
                return null;
            return Countries.FirstOrDefault(c => c.Text == ViewState.Country);
        }

        public void SetSelectedCountry(IconSelectCountry value)
        {
            if (value == null)
            {
                PasswordRecoveryViewService.SetCountry(null);
                PasswordRecoveryViewService.SetRegionCode(null);
                PasswordRecoveryViewService.SetMobilePhone(null);
            }
            else
            {
                PasswordRecoveryViewService.SetCountry(value.Text);
                _countryRegionCodes.TryGetValue(value.Text, out var regionCode);
                PasswordRecoveryViewService.SetRegionCode(regionCode);
                var prefix = value.PhonePrefix;
                if (prefix.StartsWith("+"))
                    prefix = prefix[1..];
                PasswordRecoveryViewService.SetMobilePhone(prefix);
            }
        }

        public void OnClickClearValueButtonHandler(MouseEventArgs args)
        {
            SetSelectedCountry(null);
        }

        public FieldIdentifier GetCountryFieldIdentifier()
        {
            _countryFieldIdentifier ??= new FieldIdentifier(ViewState, nameof(ViewState.Country));
            return _countryFieldIdentifier.Value;
        }

        public void OnCloseButtonClickHandler()
        {
            PasswordRecoveryViewService.Reset();
        }

        private async Task SelectRecoveryChannel(ICollection<Button> selectedItems)
        {
            var selectedChannel = selectedItems.Any(item => item.Name == "Sms")
                ? PasswordRecoveryChannel.Sms
                : PasswordRecoveryChannel.Email;

            var provider = ViewState.AvailableProviders.FirstOrDefault(item => item.Channel == selectedChannel);
            PasswordRecoveryViewService.SetActiveProvider(provider);

            if (selectedChannel == PasswordRecoveryChannel.Sms && _isLoaded && ViewState.Country is null)
                await SelectDefaultCountryAsync();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserRegisterConfigurationViewState);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            _phoneCountries = (await PhoneValidationService.GetCountriesAsync()).ToList();

            foreach (var country in _phoneCountries.OrderBy(c => c.CountryName))
            {
                _countryRegionCodes[country.CountryName] = country.RegionCode;
                _countryMasks[country.CountryName] = country.InputMask;
                Countries.Add(new IconSelectCountry
                {
                    Text = country.CountryName,
                    PhonePrefix = country.CallingCode,
                    Icon = country.Flag ?? string.Empty
                });
            }

            foreach (var item in Countries)
                item.Display = item.Text + " " + item.PhonePrefix;

            _isLoaded = true;
            await base.OnInitializedAsync();

            if (ViewState.Channel == PasswordRecoveryChannel.Sms && ViewState.Country is null)
                await SelectDefaultCountryAsync();

            await InvokeAsync(StateHasChanged);
        }

        private async Task SelectDefaultCountryAsync()
        {
            var regionCode = await ServerInfo.GetRegionCodeAsync();
            var def = CountryDefaults.ResolveDefault(regionCode, _phoneCountries);
            var match = def != null ? Countries.FirstOrDefault(c => c.Text == def.CountryName) : null;
            SetSelectedCountry(match);
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(UserRegisterConfigurationViewState);
            base.Dispose();
        }
    }
}
