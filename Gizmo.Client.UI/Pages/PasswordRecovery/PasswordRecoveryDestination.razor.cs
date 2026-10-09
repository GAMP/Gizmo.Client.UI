using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryDestinationRoute)]
    public partial class PasswordRecoveryDestination : ShellComponentBase
    {
        private FieldIdentifier? _countryFieldIdentifier;
        private RecoveryRequest _request;

        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [CascadingParameter] RecoveryHandoff Recovery { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PasswordRecoveryDestinationViewService PasswordRecoveryDestinationViewService { get; set; }

        [Inject]
        PasswordRecoveryDestinationViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        protected string ErrorText =>
            ViewState.ErrorMessage == LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_NO_METHODS_AVAILABLE))
                ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_RECOVERY_NOT_FOUND)
                : ViewState.ErrorMessage;

        private bool HoldsRequest => ViewState.IdentifierKind == PasswordRecoveryIdentifierKind.MobilePhone
            ? ViewState.MobilePhone == _request.Value
            : ViewState.MatchValue == _request.Value;

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

        public void OnCloseButtonClickHandler()
        {
            PasswordRecoveryDestinationViewService.Reset();
        }

        private void Prefill(RecoveryRequest request)
        {
            if (request is null)
                return;

            _request = request;

            if (ViewState.IdentifierKind == PasswordRecoveryIdentifierKind.MobilePhone)
            {
                PasswordRecoveryDestinationViewService.SetCountry(request.Country);
                PasswordRecoveryDestinationViewService.SetRegionCode(request.RegionCode);
                PasswordRecoveryDestinationViewService.SetMobilePhone(request.Value);
            }
            else
            {
                PasswordRecoveryDestinationViewService.SetMatchValue(request.Value);
            }

            SubmitPrefilled();
        }

        private void SubmitPrefilled()
        {
            if (_request is null || ViewState.IsLoading)
                return;

            if (!HoldsRequest)
            {
                _request = null;
                return;
            }

            if (ViewState.IsValid != true)
                return;

            _request = null;
            DispatchWorkflow(PasswordRecoveryDestinationViewService.SubmitAsync);
        }

        private void OnViewStateChanged(object sender, EventArgs e) => SubmitPrefilled();

        protected override void OnInitialized()
        {
            ViewState.OnChange += OnViewStateChanged;
            this.SubscribeChange(ViewState);
            Prefill(Recovery?.Take());
            base.OnInitialized();
        }

        public override void Dispose()
        {
            ViewState.OnChange -= OnViewStateChanged;
            this.UnsubscribeChange(ViewState);
            base.Dispose();
        }
    }
}
