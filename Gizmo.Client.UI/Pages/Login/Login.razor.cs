using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route("/")]
    public partial class Login : ComponentBase
    {
        private bool _logginIn;

        [Inject]
        NavigationManager UriHelper { get; set; }

        public async Task Navigate()
        {
            _logginIn = true;

            await Task.Delay(2000);

            UriHelper.NavigateTo("/agreement");
        }
    }
}