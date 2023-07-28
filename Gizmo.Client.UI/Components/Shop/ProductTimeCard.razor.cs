using System.Collections.Generic;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductTimeCard : CustomDOMComponentBase
    {
        //TODO: We have a problem with the rendering after content visibility by the 'mouseover' event.
        //In the good version of the implementation, this flag should be 'false' by default.
        private bool _isVisibleImageContent = true;

        private bool _clickHandled = false;
        protected bool _shouldRender;

        [Inject]
        ProductDetailsPageViewState ProductDetailsPageViewState { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        public void OpenDetails()
        {
            if (_clickHandled)
            {
                _clickHandled = false;
                return;
            }

            if (!ProductDetailsPageViewState.DisableProductDetails)
                NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={Product.Id}");
        }

        public void Ignore()
        {
            _clickHandled = true;
        }

        private Task OnVisibleImageContent(MouseEventArgs _)
        {
            if (!_isVisibleImageContent)
            {
                _isVisibleImageContent = true;
                StateHasChanged();
            }
            return Task.CompletedTask;
        }

        #region OVERRIDES

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<UserProductViewState>(nameof(Product), out var newProduct))
            {
                var productChanged = !EqualityComparer<UserProductViewState>.Default.Equals(Product, newProduct);
                if (productChanged)
                {
                    _shouldRender = true;
                }
            }

            await base.SetParametersAsync(parameters);
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"ReRender {this}");
            }
            else
            {
                //await InvokeVoidAsync("writeLine", $"Render {this.ToString()}");
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        #endregion
    }
}
