using System;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;

namespace Gizmo.Client.UI.Services
{
    internal static class PackagePurchaseFlow
    {
        private static readonly TimeSpan WAIT_CEILING = TimeSpan.FromSeconds(30);

        public static async Task RunAsync(int productId,
            ClientServerCartViewService cartService,
            UserCartViewService checkoutService)
        {
            cartService.AddProduct(productId);

            if (!await WaitForProductInCartAsync(cartService, productId))
                return;

            await checkoutService.SubmitAsync();
        }

        private static async Task<bool> WaitForProductInCartAsync(ClientServerCartViewService cartService, int productId)
        {
            var deadline = DateTime.UtcNow.Add(WAIT_CEILING);

            while (DateTime.UtcNow < deadline)
            {
                var state = cartService.ViewState;

                if (!state.IsStateUpdateRequired && !state.IsStateUpdating && state.Products.Any(a => a.ProductId == productId))
                    return true;

                await Task.Delay(200);
            }

            return false;
        }
    }
}
