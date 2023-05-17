using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using System.Linq;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.RegistrationConfirmationMethodRoute)]
    public partial class RegistrationConfirmationMethod : CustomDOMComponentBase
    {
        private bool _isLoaded;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        CountryInformationService CountryInformationService { get; set; }
        
        [Inject]
        UserRegistrationConfirmationMethodViewService UserRegistrationConfirmationMethodService { get; set; }

        [Inject]
        UserRegistrationConfirmationMethodViewState ViewState { get; set; }

        [Inject]
        UserRegistrationViewState UserRegistrationViewState { get; set; }

        [Inject]
        UserVerificationViewState UserVerificationViewState { get; set; }

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
                UserRegistrationConfirmationMethodService.SetCountry(null);
                UserRegistrationConfirmationMethodService.SetPrefix(null);
            }
            else
            {
                UserRegistrationConfirmationMethodService.SetCountry(value.Text);
                UserRegistrationConfirmationMethodService.SetPrefix(value.PhonePrefix);
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserVerificationViewState);

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var countries = await CountryInformationService.GetCountryInfoAsync();

            foreach (var country in countries)
            {
                foreach (var suffix in country.CallingCodeSuffixes)
                {
                    Countries.Add(new IconSelectCountry()
                    {
                        Text = country.NativeName,
                        PhonePrefix = country.CallingCodeRoot + suffix,
                        Icon = country.FlagSvg
                    });
                }
            }

            var other = new IconSelectCountry()
            {
                Text = "Other",
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
            this.UnsubscribeChange(UserVerificationViewState);
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
