using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;

namespace Gizmo.Client.UI.Services
{
    internal static class PackagePurchaseFlow
    {
        private static readonly TimeSpan WAIT_CEILING = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan POLL_INTERVAL = TimeSpan.FromMilliseconds(50);

        public static async Task RunAsync(int productId,
            ClientServerCartViewService cartService,
            UserCartViewService checkoutService,
            CancellationToken cancellationToken)
        {
            Guid? addedLine = null;

            if (!cartService.ViewState.Products.Any(a => a.ProductId == productId))
            {
                var knownLines = cartService.ViewState.Products.Select(a => a.Guid).ToHashSet();

                cartService.AddProduct(productId);

                addedLine = await WaitForNewLineAsync(cartService, productId, knownLines);

                if (addedLine is null)
                    return;
            }

            if (!cancellationToken.IsCancellationRequested)
                await checkoutService.SubmitAsync();

            if (addedLine is { } line && cartService.ViewState.Products.Any(a => a.Guid == line))
                cartService.RemoveEntry(line);
        }

        private static async Task<Guid?> WaitForNewLineAsync(ClientServerCartViewService cartService, int productId, HashSet<Guid> knownLines)
        {
            var deadline = DateTime.UtcNow.Add(WAIT_CEILING);
            var requestSeen = false;

            while (DateTime.UtcNow < deadline)
            {
                var state = cartService.ViewState;

                if (state.IsStateUpdateRequired || state.IsStateUpdating)
                {
                    requestSeen = true;
                }
                else
                {
                    var line = state.Products.FirstOrDefault(a => a.ProductId == productId && !knownLines.Contains(a.Guid));

                    if (line is not null)
                        return line.Guid;

                    if (requestSeen)
                        return null;
                }

                await Task.Delay(POLL_INTERVAL);
            }

            return null;
        }
    }
}
