using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
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

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryDestinationRoute)]
    public partial class PasswordRecoveryDestination : CustomDOMComponentBase
    {
        private FieldIdentifier? _countryFieldIdentifier;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PasswordRecoveryDestinationViewService PasswordRecoveryDestinationViewService { get; set; }

        [Inject]
        PasswordRecoveryDestinationViewState ViewState { get; set; }

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
                PasswordRecoveryDestinationViewService.SetCountry(null);
                PasswordRecoveryDestinationViewService.SetRegionCode(null);
                PasswordRecoveryDestinationViewService.SetMobilePhone(null);
            }
            else
            {
                PasswordRecoveryDestinationViewService.SetCountry(selection.CountryName);
                PasswordRecoveryDestinationViewService.SetRegionCode(selection.RegionCode);
                PasswordRecoveryDestinationViewService.SetMobilePhone(selection.CallingCodeDigits);
            }
            return Task.CompletedTask;
        }

        private Task OnPhoneValueChangedAsync(string? value)
        {
            PasswordRecoveryDestinationViewService.SetMobilePhone(value);
            return Task.CompletedTask;
        }

        private void SelectIdentifierKind(ICollection<Button> selectedItems)
        {
            if (selectedItems.Any(item => item.Name == nameof(PasswordRecoveryIdentifierKind.Email)))
            {
                PasswordRecoveryDestinationViewService.SetIdentifierKind(PasswordRecoveryIdentifierKind.Email);
                return;
            }

            if (selectedItems.Any(item => item.Name == nameof(PasswordRecoveryIdentifierKind.MobilePhone)))
            {
                PasswordRecoveryDestinationViewService.SetIdentifierKind(PasswordRecoveryIdentifierKind.MobilePhone);
                return;
            }

            PasswordRecoveryDestinationViewService.SetIdentifierKind(PasswordRecoveryIdentifierKind.Username);
        }

        public void OnCloseButtonClickHandler()
        {
            PasswordRecoveryDestinationViewService.Reset();
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
