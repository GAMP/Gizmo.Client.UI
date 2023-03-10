using System.Threading.Tasks;

using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductQuantityPicker : CustomDOMComponentBase
    {
        private UserCartProductItemViewState _productItemViewState;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public ButtonSizes PikerSize { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public async Task OnAddQuantityButtonClickHandlerAsync(MouseEventArgs args)
        {
            await UserCartService.AddUserCartProductAsync(ProductId, 1);
            await OnClick.InvokeAsync(args);
        }

        public async Task OnRemoveQuantityButtonClickHandler(MouseEventArgs args)
        {
            await UserCartService.RemoveUserCartProductAsync(ProductId, 1);
            await OnClick.InvokeAsync(args);
        }

        protected override async Task OnInitializedAsync()
        {
            _productItemViewState = await UserCartService.GetCartProductItemViewStateAsync(ProductId);
            this.SubscribeChange(_productItemViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(_productItemViewState);

            base.Dispose();
        }
    }
}
