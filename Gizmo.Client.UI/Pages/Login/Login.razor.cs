using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.LoginRoute)]
    public partial class Login : CustomDOMComponentBase
    {
        [Inject]
        IOptions<UserLoginOptions> UserLoginOptions { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        UserLoginViewState ViewState { get; set; }

        [Inject]
        HostQRCodeViewState HostQRCodeViewState { get; set; }

        [Inject()]
        HostReservationViewState HostReservationViewState { get; set; }

        [Inject()]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; init; }

        [Inject()]
        UserLoginConfigurationViewState UserLoginConfigurationViewState { get; init; }

        [Inject]
        HostLockViewService HostUserLockService { get; set; }

        [Inject]
        UserPasswordRecoveryMethodServiceViewState UserPasswordRecoveryMethodServiceViewState { get; set; }

        private Task OnKeyPressHandle(KeyboardEventArgs args)
        {
            if (args.Key == "Enter")
            {
                return UserLoginService.LoginAsync();
            }

            return Task.CompletedTask;
        }

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

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(HostQRCodeViewState);

            //await InvokeVoidAsync("navigationBlock");

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(HostQRCodeViewState);

            base.Dispose();
        }
    }
}
