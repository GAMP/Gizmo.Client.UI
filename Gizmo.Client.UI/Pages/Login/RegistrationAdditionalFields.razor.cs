using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.RegistrationAdditionalFieldsRoute)]
    public partial class RegistrationAdditionalFields : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        CountryInformationService CountryInformationService { get; set; }

        [Inject]
        UserRegistrationViewState UserRegistrationViewState { get; set; }

        [Inject]
        UserRegistrationConfirmationMethodViewStateService UserRegistrationConfirmationMethodService { get; set; }

        [Inject]
        UserRegistrationConfirmationMethodViewState UserRegistrationConfirmationMethodViewState { get; set; }

        [Inject]
        UserRegistrationAdditionalFieldsViewStateService UserRegistrationAdditionalFieldsService { get; set; }

        [Inject]
        UserRegistrationAdditionalFieldsViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public List<IconSelectCountry> Countries { get; set; }

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
                UserRegistrationAdditionalFieldsService.SetCountry(null);
                UserRegistrationAdditionalFieldsService.SetPrefix(null);
            }
            else
            {
                UserRegistrationAdditionalFieldsService.SetCountry(value.Text);
                UserRegistrationAdditionalFieldsService.SetPrefix(value.PhonePrefix);
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
                Countries.Add(new IconSelectCountry()
                {
                    Text = country.NativeName,
                    PhonePrefix = country.CallingCodeRoot + (country.CallingCodeSuffixes.Count() == 1 ? country.CallingCodeSuffixes.First() : ""),
                    Icon = country.FlagSvg
                });
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

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
