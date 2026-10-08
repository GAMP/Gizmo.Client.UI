using Gizmo.Client.UI.Localization.Resources;
using System;
using System.Threading.Tasks;
using Gizmo;
using Gizmo.Client;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationRedirectRoute)]
    public partial class UserRegistrationRedirect : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationRedirectViewService RegistrationRedirectViewService { get; set; }

        [Inject]
        UserRegistrationRedirectViewState ViewState { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        public void OnCloseErrorHandler()
        {
            RegistrationRedirectViewService.Reset();
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

        private string GetProviderDisplayName()
        {
            return RegistrationSession.SelectedProvider?.Name ?? string.Empty;
        }

        protected string QrCardClass => ViewState.IsQrExpired
            ? "giz-signin__qr-card giz-signin__qr-card--expired"
            : "giz-signin__qr-card";

        protected string QrTitle => ViewState.IsQrExpired
            ? LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_REDIRECT_QR_EXPIRED))
            : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_SIGNUP_QR_WAIT);

        protected string QrText => ViewState.IsQrExpired
            ? LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_REDIRECT_SESSION_EXPIRED))
            : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_SIGNUP_QR_WAIT_HINT);
    }
}
