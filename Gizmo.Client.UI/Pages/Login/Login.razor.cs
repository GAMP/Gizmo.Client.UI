using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route("/")]
    public partial class Login : CustomDOMComponentBase
    {
        private bool _logginIn;

        [Inject]
        UserLoginService UserLoginService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserLoginService.ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserLoginService.ViewState);
            base.Dispose();
        }
    }
}