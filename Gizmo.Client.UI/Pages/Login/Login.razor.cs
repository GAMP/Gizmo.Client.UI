using Gizmo.Client.Options;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
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
        private bool _isLoaded;
        private readonly Dictionary<string, string> _countryRegionCodes = new();
        private readonly Dictionary<string, string?> _countryMasks = new();
        private FieldIdentifier? _countryFieldIdentifier;

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
        IPhoneValidationService PhoneValidationService { get; set; }

        public List<IconSelectCountry> Countries { get; set; } = new();

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
                UserLoginService.SetCountry(null);
                UserLoginService.SetRegionCode(null);
                UserLoginService.SetLoginName(string.Empty);
            }
            else
            {
                UserLoginService.SetCountry(value.Text);
                _countryRegionCodes.TryGetValue(value.Text, out var regionCode);
                UserLoginService.SetRegionCode(regionCode);

                if (ViewState.LoginType == View.UserLoginType.MobilePhone)
                    UserLoginService.SetLoginName(GetPrefixDigits(value));
            }
        }

        private static string GetPrefixDigits(IconSelectCountry value)
        {
            var prefix = value.PhonePrefix;
            if (prefix.StartsWith("+"))
                prefix = prefix.Substring(1);
            return prefix;
        }

        public int GetLockedPrefixLength()
        {
            var selected = GetSelectedCountry();
            if (selected == null || string.IsNullOrEmpty(selected.PhonePrefix))
                return 0;

            return selected.PhonePrefix.Count(char.IsDigit);
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

        private Task OnKeyDownHandle(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                return UserLoginService.LoginAsync();
            }

            return Task.CompletedTask;
        }

        private void SelectLoginType(ICollection<Button> selectedItems)
        {
            if (selectedItems.Where(a => a.Name == "Username").Any())
            {
                UserLoginService.SetLoginMethod(View.UserLoginType.UsernameOrEmail);
            }
            else
            {
                var switching = ViewState.LoginType != View.UserLoginType.MobilePhone;
                UserLoginService.SetLoginMethod(View.UserLoginType.MobilePhone);

                if (switching)
                {
                    var selected = GetSelectedCountry();
                    if (selected != null)
                        UserLoginService.SetLoginName(GetPrefixDigits(selected));
                }
            }
        }

        public void OnCloseButtonClickHandler()
        {
            UserLoginService.Reset();
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(HostQRCodeViewState);
            this.SubscribeChange(HostReservationViewState);

            var countries = await PhoneValidationService.GetCountriesAsync();

            foreach (var country in countries.OrderBy(c => c.CountryName))
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
