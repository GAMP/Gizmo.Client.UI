using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserDepositsRoute)]
    public partial class Deposits : CustomDOMComponentBase
    {
        [Inject]
        DepositTransactionsService DepositTransactionsService { get; set; }

        [Inject]
        DepositTransactionsViewState ViewState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await DepositTransactionsService.LoadDepositTransactionsAsync();

            await base.OnInitializedAsync();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
