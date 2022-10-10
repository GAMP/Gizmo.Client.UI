using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuUserActions : CustomDOMComponentBase
    {
        [Inject]
        UserService UserService { get; set; }

        [Inject]
        UserLockService UserLockService { get; set; }

        public bool TopUpIsOpen { get; set; }

        private void OnClickTopUpButtonHandler()
        {
            TopUpIsOpen = true;
        }
    }
}