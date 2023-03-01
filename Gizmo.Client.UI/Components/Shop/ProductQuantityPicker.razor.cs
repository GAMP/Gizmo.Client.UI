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
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public ButtonSizes PikerSize { get; set; } = ButtonSizes.Medium;

        UserCartProductItemViewState ProductItemViewState { get; set; }

        public Task OnAddQuantityButtonClickHandlerAsync(MouseEventArgs _) => 
            UserCartService.AddProductAsync(ProductId, 1);
        public Task OnRemoveQuantityButtonClickHandler(MouseEventArgs _) => 
            UserCartService.RemoveProductAsync(ProductId, 1);

        protected override async Task OnInitializedAsync()
        {
            ProductItemViewState = await UserCartService.GetCartProductItemViewStateAsync(ProductId);

            this.SubscribeChange(ProductItemViewState);
            base.OnInitialized();
        }
        public override void Dispose()
        {
            this.UnsubscribeChange(ProductItemViewState);
            base.Dispose();
        }
    }
}
