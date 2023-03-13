using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.UserProductsRoute)]
    public partial class Products : CustomDOMComponentBase
    {
        [Inject]
        TimeProductsService TimeProductsService { get; set; }

        [Inject]
        TimeProductsViewState ViewState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await TimeProductsService.LoadTimeProductsAsync();

            this.SubscribeChange(ViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
