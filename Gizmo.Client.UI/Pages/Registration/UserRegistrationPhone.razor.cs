using System.Linq;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Microsoft.AspNetCore.Components.Web;
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

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [CascadingParameter]
        RegistrationCardContext Card { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        protected string PhoneError => RegistrationPhoneViewService.EditContext
            .GetValidationMessages(new FieldIdentifier(ViewState, nameof(ViewState.MobilePhone)))
            .FirstOrDefault();

        private Task OnKeyDownAsync(KeyboardEventArgs args)
        {
            if (args.Key != "Enter" || ViewState.IsLoading || ViewState.IsValid != true)
                return Task.CompletedTask;

            return RegistrationPhoneViewService.SubmitAsync();
        }

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

        private Task BackAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            Card?.SetBack(this, BackAsync);
            this.SubscribeChange(ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            Card?.ClearBack(this);
            this.UnsubscribeChange(ViewState);
            base.Dispose();
        }
    }
}
