using Gizmo.Client.UI.View.Services;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Pages
{
    public partial class ProductsProductProperties : CustomDOMComponentBase
    {
        private UserProductViewState _userProductViewState;

        [Inject]
        public UserProductViewState UserProductViewState
        {
            get { return _userProductViewState; }
            private set { _userProductViewState = value; }
        }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public DateTime? FirstUsageTime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _userProductViewState = await UserProductViewStateLookupService.GetStateAsync(ProductId);

            if (_userProductViewState != null)
            {
                this.SubscribeChange(_userProductViewState);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_userProductViewState != null)
            {
                this.UnsubscribeChange(_userProductViewState);
            }

            base.Dispose();
        }
    }
}
