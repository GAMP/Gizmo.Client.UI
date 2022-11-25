using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public partial class PasswordRecovery : ComponentBase
    {
        [Inject]
        UserPasswordRecoveryService UserPasswordRecoveryService { get; set; }

        [Inject]
        UserLoginService UserLoginService { get; set; }

        [Inject]
        HostService HostService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        private void SelectRecoveryMethod(ICollection<Button> selectedItems)
        {
            if (selectedItems.Where(a => a.Name == "Email").Any())
                UserPasswordRecoveryService.SetRecoveryMethod(View.UserPasswordRecoveryMethod.Email);
            else
                UserPasswordRecoveryService.SetRecoveryMethod(View.UserPasswordRecoveryMethod.MobilePhone);
        }

    }
}
