using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserDepositsRoute)]
    public partial class Deposits : ComponentBase
    {
        [Inject]
        DepositTransactionsService DepositTransactionsService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await DepositTransactionsService.LoadDepositTransactionsAsync();

            await base.OnInitializedAsync();
        }
    }
}
