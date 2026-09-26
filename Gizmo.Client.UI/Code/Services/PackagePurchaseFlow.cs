using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    internal static class PackagePurchaseFlow
    {
        private static readonly TimeSpan WAIT_CEILING = TimeSpan.FromSeconds(90);

        public static async Task<bool> RunAsync(int productId,
            ClientServerCartViewService cartService,
            IClientDialogService dialogService)
        {
            if (dialogService is not ClientDialogService dialogs)
                return false;

            var before = cartService.ViewState.Products.Select(a => a.Guid).ToHashSet();

            cartService.AddProduct(productId);

            var entryId = await WaitForCartEntryAsync(cartService, before);

            if (entryId is null)
                return false;

            var dialog = await dialogs.ShowPackagePurchaseDialogAsync(productId, entryId.Value);

            if (dialog.Result == AddComponentResultCode.Opened)
                await dialog.WaitForResultAsync();

            return true;
        }

        private static async Task<Guid?> WaitForCartEntryAsync(ClientServerCartViewService cartService, HashSet<Guid> before)
        {
            var deadline = DateTime.UtcNow.Add(WAIT_CEILING);

            while (DateTime.UtcNow < deadline)
            {
                var entry = cartService.ViewState.Products.FirstOrDefault(a => !before.Contains(a.Guid));

                if (entry is not null)
                    return entry.Guid;

                await Task.Delay(200);
            }

            return null;
        }
    }
}
