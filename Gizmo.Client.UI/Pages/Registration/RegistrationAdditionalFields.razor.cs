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

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.RegistrationAdditionalFieldsRoute)]
    public partial class RegistrationAdditionalFields : CustomDOMComponentBase
    {
        private bool _isLoaded;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        CountryInformationService CountryInformationService { get; set; }

        [Inject]
        UserRegistrationViewState UserRegistrationViewState { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        UserRegistrationAdditionalFieldsViewService UserRegistrationAdditionalFieldsViewService { get; set; }

        [Inject]
        UserRegistrationAdditionalFieldsViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public void OnCloseButtonClickHandler()
        {
            UserRegistrationAdditionalFieldsViewService.Reset();
        }

        public List<IconSelectCountry> Countries { get; set; } = new List<IconSelectCountry>();

        public void OnClickClearValueButtonHandler(MouseEventArgs args)
        {
            SetSelectedCountry(Countries.Where(a => a.PhonePrefix == "+").FirstOrDefault());
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
                UserRegistrationAdditionalFieldsViewService.SetCountry(null);
            }
            else
            {
                UserRegistrationAdditionalFieldsViewService.SetCountry(value.Text);
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var countries = await CountryInformationService.GetCountryInfoAsync();

            foreach (var country in countries)
            {
                if (country.CallingCodeSuffixes.Count() == 1)
                {
                    Countries.Add(new IconSelectCountry()
                    {
                        Text = country.NativeName,
                        PhonePrefix = country.CallingCodeRoot + country.CallingCodeSuffixes.FirstOrDefault(),
                        Icon = country.FlagSvg
                    });
                }
                else
                {
                    Countries.Add(new IconSelectCountry()
                    {
                        Text = country.NativeName,
                        PhonePrefix = country.CallingCodeRoot,
                        Icon = country.FlagSvg
                    });
                }
            }

            var other = new IconSelectCountry()
            {
                Text = LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_COUNTRY_OTHER)),
                PhonePrefix = "+",
                Icon = "_content/Gizmo.Client.UI/img/no-flag-image.svg"
            };

            Countries.Add(other);

            foreach (var item in Countries)
            {
                item.Display = item.Text + " " + item.PhonePrefix;
            }

            SetSelectedCountry(other);
            _isLoaded = true;
            await InvokeAsync(StateHasChanged);

            var defaultCountry = await CountryInformationService.GetCurrentCountryInfoAsync();
            IconSelectCountry defaultItem = null;

            if (defaultCountry != null && defaultCountry.CallingCodeSuffixes.Count() > 0)
            {
                defaultItem = Countries.Where(a => a.PhonePrefix == defaultCountry.CallingCodeRoot + defaultCountry.CallingCodeSuffixes.First()).FirstOrDefault();
            }

            if (defaultItem != null && ViewState.Country == other.Text)
            {
                SetSelectedCountry(defaultItem);
                await InvokeAsync(StateHasChanged);
            }

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
