using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class CheckoutDialogProductUnitPrice : CustomDOMComponentBase
    {
        private UserProductViewState _userProductViewState;

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _userProductViewState = await UserProductViewStateLookupService.GetStateAsync(ProductId);
            this.SubscribeChange(_userProductViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_userProductViewState);

            base.Dispose();
        }
    }
}
