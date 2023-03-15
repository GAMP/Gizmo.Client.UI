using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearchResultCardProductGroup : CustomDOMComponentBase
    {
        private UserProductGroupViewState _userProductGroupViewState;

        [Inject]
        UserProductGroupViewStateLookupService UserProductGroupViewStateLookupService { get; set; }

        [Parameter]
        public int ProductGroupId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _userProductGroupViewState = await UserProductGroupViewStateLookupService.GetStateAsync(ProductGroupId);
            this.SubscribeChange(_userProductGroupViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_userProductGroupViewState);

            base.Dispose();
        }
    }
}
