using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationPhoneRoute)]
    public partial class UserRegistrationPhone : CustomDOMComponentBase
    {
        private FieldIdentifier? _countryFieldIdentifier;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationPhoneViewService RegistrationPhoneViewService { get; set; }

        [Inject]
        UserRegistrationPhoneViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        private FieldIdentifier GetCountryFieldIdentifier()
        {
            if (_countryFieldIdentifier == null)
                _countryFieldIdentifier = new FieldIdentifier(ViewState, nameof(ViewState.Country));
            return _countryFieldIdentifier.Value;
        }

        private Task OnCountryChangedAsync(PhoneCountrySelection? selection)
        {
            if (selection == null)
            {
                RegistrationPhoneViewService.SetCountry(null);
                RegistrationPhoneViewService.SetRegionCode(null);
                RegistrationPhoneViewService.SetMobilePhone(null);
            }
            else
            {
                RegistrationPhoneViewService.SetCountry(selection.CountryName);
                RegistrationPhoneViewService.SetRegionCode(selection.RegionCode);
                RegistrationPhoneViewService.SetMobilePhone(selection.CallingCodeDigits);
            }
            return Task.CompletedTask;
        }

        private Task OnPhoneValueChangedAsync(string? value)
        {
            RegistrationPhoneViewService.SetMobilePhone(value);
            return Task.CompletedTask;
        }

        public void OnCloseButtonClickHandler()
        {
            RegistrationPhoneViewService.Reset();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            base.Dispose();
        }
    }
}
