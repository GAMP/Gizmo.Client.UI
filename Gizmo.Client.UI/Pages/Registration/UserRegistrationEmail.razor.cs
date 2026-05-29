using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationEmailRoute)]
    public partial class UserRegistrationEmail : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationEmailViewService RegistrationEmailViewService { get; set; }

        [Inject]
        UserRegistrationEmailViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public void OnCloseButtonClickHandler()
        {
            RegistrationEmailViewService.Reset();
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
