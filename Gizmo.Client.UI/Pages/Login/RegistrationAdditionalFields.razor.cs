using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
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
        UserRegistrationConfirmationMethodViewService UserRegistrationConfirmationMethodService { get; set; }

        [Inject]
        UserRegistrationConfirmationMethodViewState UserRegistrationConfirmationMethodViewState { get; set; }

        [Inject]
        UserRegistrationAdditionalFieldsViewService UserRegistrationAdditionalFieldsViewService { get; set; }

        [Inject]
        UserRegistrationAdditionalFieldsViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public string GetMask()
        {
            var selectedCountry = GetSelectedCountry();

            if (selectedCountry != null)
            {
                if (selectedCountry.PhonePrefix.Length - 1 > 0)
                {
                    return new string('#', selectedCountry.PhonePrefix.Length - 1) + "-###-###-####";
                }
            }

            return "###-###-####";
        }

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
                UserRegistrationConfirmationMethodService.SetMobilePhone(null);
            }
            else
            {
                UserRegistrationAdditionalFieldsViewService.SetCountry(value.Text);
                var tmp = value.PhonePrefix;
                if (tmp.StartsWith("+"))
                {
                    tmp = tmp.Substring(1);
                }
                UserRegistrationConfirmationMethodService.SetMobilePhone(tmp);
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
                Text = LocalizationService.GetString("GIZ_COUNTRY_OTHER"),
                PhonePrefix = "+",
                Icon = "_content/Gizmo.Client.UI/img/no-flag-image.svg"
            };

            Countries.Add(other);

            foreach (var item in Countries)
            {
                item.Display = item.Text + " " + item.PhonePrefix;
            }

            //Render the list first.
            await InvokeAsync(StateHasChanged);

            IconSelectCountry defaultItem = null;
            var defaultCountry = await CountryInformationService.GetCurrentCountryInfoAsync();

            if (defaultCountry != null && defaultCountry.CallingCodeSuffixes.Count() > 0)
            {
                defaultItem = Countries.Where(a => a.PhonePrefix == defaultCountry.CallingCodeRoot + defaultCountry.CallingCodeSuffixes.First()).FirstOrDefault();
            }

            if (defaultItem == null)
                defaultItem = other;

            SetSelectedCountry(defaultItem);

            _isLoaded = true;

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
