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
        UserPasswordRecoveryService UserPasswordRecoveryService { get; set; }

        [Inject]
        UserPasswordRecoveryViewState ViewState { get; set; }

        [Inject]
        UserLoginService UserLoginService { get; set; }

        [Inject]
        HostService HostService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        private void SelectRecoveryMethod(ICollection<Button> selectedItems)
        {
            if (selectedItems.Where(a => a.Name == "Email").Any())
                UserPasswordRecoveryService.SetSelectedRecoveryMethod(View.UserPasswordRecoveryMethod.Email);
            else
                UserPasswordRecoveryService.SetSelectedRecoveryMethod(View.UserPasswordRecoveryMethod.MobilePhone);
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
