using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Services;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public partial class PasswordRecovery : CustomDOMComponentBase
    {
        private FieldIdentifier? _countryFieldIdentifier;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

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

        private FieldIdentifier GetCountryFieldIdentifier()
        {
            _countryFieldIdentifier ??= new FieldIdentifier(ViewState, nameof(ViewState.Country));
            return _countryFieldIdentifier.Value;
        }

        private Task OnCountryChangedAsync(PhoneCountrySelection? selection)
        {
            if (selection == null)
            {
                PasswordRecoveryViewService.SetCountry(null);
                PasswordRecoveryViewService.SetRegionCode(null);
                PasswordRecoveryViewService.SetMobilePhone(null);
            }
            else
            {
                PasswordRecoveryViewService.SetCountry(selection.CountryName);
                PasswordRecoveryViewService.SetRegionCode(selection.RegionCode);
                PasswordRecoveryViewService.SetMobilePhone(selection.CallingCodeDigits);
            }
            return Task.CompletedTask;
        }

        private Task OnPhoneValueChangedAsync(string? value)
        {
            PasswordRecoveryViewService.SetMobilePhone(value);
            return Task.CompletedTask;
        }

        public void OnCloseButtonClickHandler()
        {
            PasswordRecoveryViewService.Reset();
        }

        private Task SelectRecoveryChannel(ICollection<Button> selectedItems)
        {
            var selectedChannel = selectedItems.Any(item => item.Name == "Sms")
                ? PasswordRecoveryChannel.Sms
                : PasswordRecoveryChannel.Email;

            var provider = ViewState.AvailableProviders.FirstOrDefault(item => item.Channel == selectedChannel);
            PasswordRecoveryViewService.SetActiveProvider(provider);

            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserRegisterConfigurationViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(UserRegisterConfigurationViewState);
            base.Dispose();
        }
    }
}
