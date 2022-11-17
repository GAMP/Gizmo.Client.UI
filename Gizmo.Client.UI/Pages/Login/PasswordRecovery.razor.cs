using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Pages
{
    [Route("/passwordrecovery")]
    public partial class PasswordRecovery : ComponentBase
    {
        [Inject]
        UserPasswordRecoveryService UserPasswordRecoveryService { get; set; }

        private void SelectRecoveryMethod(ICollection<Button> selectedItems)
        {
            if (selectedItems.Where(a => a.Name == "Email").Any())
                UserPasswordRecoveryService.SetRecoveryMethod(View.UserPasswordRecoveryMethod.Email);
            else
                UserPasswordRecoveryService.SetRecoveryMethod(View.UserPasswordRecoveryMethod.MobilePhone);
        }

    }
}
