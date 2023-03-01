using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
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
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        int Quantity { get; set; }

        public async Task OnAddQuantityButtonClickHandler(MouseEventArgs _)
        {
            //Quantity++;
            await UserCartService.AddProductAsyc(ProductId, 1);
        }
        public async Task OnRemoveQuantityButtonClickHandler(MouseEventArgs _)
        {
            //Quantity--;
            await UserCartService.RemoveProductAsync(ProductId, 1);
        }

        private void UpdateQuantity(object _, EventArgs __) =>
            Quantity = UserCartService.ViewState.UserCartProducts.TryGetValue(ProductId, out var product) ? product.Quantity : 0;

        protected override void OnInitialized()
        {
            UpdateQuantity(this, EventArgs.Empty);

            UserCartService.ViewState.UserCartProducts[ProductId].OnChange += UpdateQuantity;
            base.OnInitialized();
        }
        public override void Dispose()
        {
            UserCartService.ViewState.UserCartProducts[ProductId].OnChange -= UpdateQuantity;
            base.Dispose();
        }
    }
}
