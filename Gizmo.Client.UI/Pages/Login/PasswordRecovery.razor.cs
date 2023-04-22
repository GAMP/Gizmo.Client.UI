using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public partial class PasswordRecovery : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserPasswordRecoveryViewService UserPasswordRecoveryService { get; set; }

        [Inject]
        UserVerificationViewState UserVerificationViewState { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        UserPasswordRecoveryViewState ViewState { get; set; }

        [Inject()]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState
        {
            get;init;
        }

        [Inject]
        NavigationService NavigationService { get; set; }

        private void SelectRecoveryMethod(ICollection<Button> selectedItems)
        {
            if (selectedItems.Where(a => a.Name == "Email").Any())
                UserPasswordRecoveryService.SetSelectedRecoveryMethod(UserRecoveryMethod.Email);
            else
                UserPasswordRecoveryService.SetSelectedRecoveryMethod(UserRecoveryMethod.Mobile);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserVerificationViewState);
            this.SubscribeChange(UserRegisterConfigurationViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserVerificationViewState);
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(UserRegisterConfigurationViewState);

            base.Dispose();
        }
    }
}
