using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using System.Linq;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.RegistrationConfirmationMethodRoute)]
    public partial class RegistrationConfirmationMethod : CustomDOMComponentBase
    {
        public RegistrationConfirmationMethod()
        {
        }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        CountryInformationService CountryInformationService { get; set; }
        
        [Inject]
        UserRegistrationConfirmationMethodViewStateService UserRegistrationConfirmationMethodService { get; set; }

        [Inject]
        UserRegistrationConfirmationMethodViewState ViewState { get; set; }

        [Inject]
        UserRegistrationViewState UserRegistrationViewState { get; set; }

        [Inject]
        UserVerificationViewState UserVerificationViewState { get; set; }

        [Inject()]
        IClientDialogService DialogService { get; set; }

        public List<IconSelectCountry> Countries { get; set; } = new List<IconSelectCountry>();

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
                Countries.Add(new IconSelectCountry()
                {
                    Text = country.NativeName,
                    PhonePrefix = country.CallingCodeRoot + (country.CallingCodeSuffixes.Count() == 1 ? country.CallingCodeSuffixes.First() : ""),
                    //Icon = country.FlagSvg
                });
            }

            Countries.Add(new IconSelectCountry()
            {
                Text = "Other",
                PhonePrefix = "+",
                Icon = "<svg width=\"16\" height=\"16\" viewBox=\"0 0 16 16\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><g clip-path=\"url(#clip0_2031_59693)\"><path d=\"M8 16C12.4183 16 16 12.4183 16 8C16 3.58172 12.4183 0 8 0C3.58172 0 0 3.58172 0 8C0 12.4183 3.58172 16 8 16Z\" fill=\"white\"/><path d=\"M16 7.99996C16 4.56025 13.829 1.6279 10.7826 0.497559V15.5024C13.829 14.372 16 11.4397 16 7.99996Z\" fill=\"white\"/><path d=\"M0 7.99996C0 11.4397 2.171 14.372 5.21741 15.5024V0.497559C2.171 1.6279 0 4.56025 0 7.99996Z\" fill=\"white\"/></g><defs><clipPath id=\"clip0_2031_59693\"><rect width=\"16\" height=\"16\" fill=\"white\"/></clipPath></defs></svg>"
            });

            foreach (var item in Countries)
            {
                item.Display = item.Text + " " + item.PhonePrefix;
            }

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
