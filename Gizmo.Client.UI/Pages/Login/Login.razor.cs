using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.LoginRoute)]
    public partial class Login : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        UserLoginViewState ViewState { get; set; }

        [Inject]
        HostQRCodeViewState HostQRCodeViewState { get; set; }

        [Inject()]
        HostReservationViewState HostReservationViewState 
        {
            get; 
            set;
        }

        [Inject()]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState
        {
            get;init;
        }

        [Inject()]
        UserLoginConfigurationViewState UserLoginConfigurationViewState
        {
            get;init;
        }

        [Inject]
        HostLockViewService HostUserLockService { get; set; }


        [Inject]
        UserPasswordRecoveryMethodServiceViewState UserPasswordRecoveryMethodServiceViewState { get; set; }

        private void SelectLoginType(ICollection<Button> selectedItems)
        {
            if (selectedItems.Where(a => a.Name == "Username").Any())
                UserLoginService.SetLoginMethod(View.UserLoginType.UsernameOrEmail);
            else
                UserLoginService.SetLoginMethod(View.UserLoginType.MobilePhone);
        }

        public void OnCloseButtonClickHandler()
        {
            UserLoginService.Reset();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(HostQRCodeViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(HostQRCodeViewState);

            base.Dispose();
        }
    }
}
