using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
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

        private Task SelectRecoveryProvider(ICollection<Button> selectedItems)
        {
            var selectedItem = selectedItems.FirstOrDefault();
            if (selectedItem is null || !Guid.TryParse(selectedItem.Name, out var providerPublicId))
                return Task.CompletedTask;

            var provider = ViewState.AvailableProviders.FirstOrDefault(item => item.PublicId == providerPublicId);
            PasswordRecoveryViewService.SetActiveProvider(provider);

            return Task.CompletedTask;
        }

        private string GetProviderDisplayName(PasswordRecoveryProvider provider)
        {
            return provider.Channel == PasswordRecoveryChannel.Sms
                ? LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_PHONE_NUMBER))
                : LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_EMAIL_ADDRESS));
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
