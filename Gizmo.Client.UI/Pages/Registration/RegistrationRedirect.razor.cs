using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationRedirectRoute)]
    public partial class RegistrationRedirect : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        RegistrationRedirectViewService RegistrationRedirectViewService { get; set; }

        [Inject]
        RegistrationRedirectViewState ViewState { get; set; }

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
    }
}
