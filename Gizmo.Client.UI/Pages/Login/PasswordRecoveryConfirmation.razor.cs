using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryConfirmationRoute)]
    public partial class PasswordRecoveryConfirmation : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserPasswordRecoveryConfirmationService UserPasswordRecoveryConfirmationService { get; set; }

        [Inject]
        UserPasswordRecoveryConfirmationViewState ViewState { get; set; }

        [Inject]
        UserPasswordRecoveryViewState UserPasswordRecoveryViewState { get; set; }

        [Inject]
        UserLoginService UserLoginService { get; set; }

        [Inject]
        HostService HostService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

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
