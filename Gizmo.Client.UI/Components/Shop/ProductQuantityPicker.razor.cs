using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductQuantityPicker : CustomDOMComponentBase
    {
        [Parameter]
        public UserCartProductViewState UserCartProductViewState { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Parameter]
        public ButtonSizes Size { get; set; } = ButtonSizes.Medium;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public Task OnAddQuantityButtonClickHandler(MouseEventArgs args)
        {
            return UserCartService.AddProductAsyc(UserCartProductViewState.ProductId, 1);
        }

        public Task OnRemoveQuantityButtonClickHandler(MouseEventArgs args)
        {
            return UserCartService.RemoveProductAsync(UserCartProductViewState.ProductId, 1);
        }

        public Task OnClickHandler(MouseEventArgs args)
        {
            return OnClick.InvokeAsync(args);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserCartProductViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserCartProductViewState);
            base.Dispose();
        }
    }
}