using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route("/")]
    public partial class Login : ComponentBase
    {
        private bool _logginIn;

        public async Task Navigate()
        {
            _logginIn = true;

            await Task.Delay(2000);

            UriHelper.NavigateTo("/agreement");
        }
    }
}
