using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/")]
    public partial class Login : ComponentBase
    {
        private bool _logginIn;

        [Inject]
        UserLoginService UserLoginService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserLoginService.ViewState);
            base.OnInitialized();
        }
    }
}