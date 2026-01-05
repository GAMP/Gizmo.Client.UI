using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearchProductResultCard : CustomDOMComponentBase
    {
        private UserProductViewState _userProductViewState;

        private bool _clickHandled = false;

        protected bool _shouldRender;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Inject]
        ClientServerCartViewService ClientServerCartViewService { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        private void OnClickHandler()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={ProductId.ToString()}");
        }

        private void OnClickActionButtonHandler()
        {
            _clickHandled = true;

            ClientServerCartViewService.AddProduct(ProductId);
        }

        #region OVERRIDES

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            //TODO: AAAAA

            _userProductViewState = await UserProductViewStateLookupService.GetStateAsync(ProductId);

            if (_userProductViewState != null)            
                this.SubscribeChange(_userProductViewState);
            

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_userProductViewState != null)            
                this.UnsubscribeChange(_userProductViewState);            

            base.Dispose();
        }

        #endregion
    }
}
