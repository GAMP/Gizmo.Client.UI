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
        private static readonly TimeSpan PROMPT_GRACE = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan SETTLE_MINIMUM = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan WAIT_CEILING = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan LATE_CEILING = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan POLL_INTERVAL = TimeSpan.FromMilliseconds(50);

        public static bool IsCartBusy(ClientServerCartViewService cartService) =>
            cartService.ViewState.IsStateUpdateRequired || cartService.ViewState.IsStateUpdating;

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

                var (line, requestSeen) = await WaitForNewLineAsync(cartService, productId, knownLines, PROMPT_GRACE, WAIT_CEILING);

                if (line is null)
                {
                    if (!requestSeen)
                        _ = FinishLateAsync(productId, cartService, checkoutService, knownLines, cancellationToken);

                    return;
                }

                addedLine = line;
            }

            await CheckoutAsync(cartService, checkoutService, addedLine, cancellationToken);
        }

        private static async Task CheckoutAsync(ClientServerCartViewService cartService,
            UserCartViewService checkoutService,
            Guid? addedLine,
            CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
                await checkoutService.SubmitAsync();

            if (addedLine is { } line && cartService.ViewState.Products.Any(a => a.Guid == line))
                cartService.RemoveEntry(line);
        }

        // The vendor asks before adding a package outside its hours; the answer can take longer
        // than the button should stay busy, so the line is waited for here instead.
        private static async Task FinishLateAsync(int productId,
            ClientServerCartViewService cartService,
            UserCartViewService checkoutService,
            HashSet<Guid> knownLines,
            CancellationToken cancellationToken)
        {
            try
            {
                var (line, _) = await WaitForNewLineAsync(cartService, productId, knownLines, LATE_CEILING, LATE_CEILING);

                if (line is not null)
                    await CheckoutAsync(cartService, checkoutService, line, cancellationToken);
            }
            catch (Exception)
            {
            }
        }

        private static async Task<(Guid? Line, bool RequestSeen)> WaitForNewLineAsync(ClientServerCartViewService cartService,
            int productId,
            HashSet<Guid> knownLines,
            TimeSpan promptGrace,
            TimeSpan ceiling)
        {
            var started = DateTime.UtcNow;
            var requestSeen = false;

            while (DateTime.UtcNow - started < ceiling)
            {
                var elapsed = DateTime.UtcNow - started;

                if (IsCartBusy(cartService))
                {
                    requestSeen = true;
                }
                else
                {
                    var line = cartService.ViewState.Products.FirstOrDefault(a => a.ProductId == productId && !knownLines.Contains(a.Guid));

                    if (line is not null)
                        return (line.Guid, true);

                    if (requestSeen && elapsed >= SETTLE_MINIMUM)
                        return (null, true);

                    if (!requestSeen && elapsed >= promptGrace)
                        return (null, false);
                }

                await Task.Delay(POLL_INTERVAL);
            }

            return (null, requestSeen);
        }
    }
}
