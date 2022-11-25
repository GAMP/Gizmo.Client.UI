using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserProductsRoute)]
    public partial class Products : ComponentBase
    {
        [Inject]
        TimeProductsService TimeProductsService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await TimeProductsService.LoadTimeProductsAsync();

            await base.OnInitializedAsync();
        }
    }
}
