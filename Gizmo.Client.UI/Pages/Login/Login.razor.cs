using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route("/")]
    public partial class Login : ComponentBase
    {
        private bool _logginIn;

        [Inject]
        NavigationManager UriHelper { get; set; }

        [Inject]
        UserLoginService UserLoginService { get; set; }

        [Inject]
        UserLoginViewState UserLoginViewState { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserLoginViewState);
            base.OnInitialized();
        }

        public async Task Navigate()
        {
            _logginIn = true;

            await Task.Delay(2000);

            UriHelper.NavigateTo("/agreement");
        }
    }
}