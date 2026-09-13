using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Buys a time package in one step: add it to the cart, then open the purchase dialog
    /// on that entry.
    /// </summary>
    /// <remarks>
    /// Shared because there are two entry points - the buy button on the home board and the
    /// one on a package in the shop - and the ordering below is too easy to get wrong to
    /// duplicate. Static rather than a service: it holds no state, and registering another
    /// service in the skin's composition is one more thing that can fail to load.
    /// </remarks>
    internal static class PackagePurchaseFlow
    {
        /// <summary>
        /// How long to wait for the cart entry to appear.
        /// </summary>
        /// <remarks>
        /// Sized for how long somebody may take to answer a confirmation prompt, not for
        /// the request, which completes in a fraction of a second.
        /// </remarks>
        private static readonly TimeSpan WAIT_CEILING = TimeSpan.FromSeconds(90);

        /// <summary>
        /// Adds the package to the cart and opens the purchase dialog on that entry.
        /// </summary>
        /// <remarks>
        /// The order matters. Before adding a time package that is outside its usage
        /// availability window, <c>ClientServerCartViewService.ValidateRequestAsync</c>
        /// raises the stock "not available right now, add anyway?" prompt and waits for an
        /// answer. Dialogs are a queue, not a stack: with our dialog already open that
        /// prompt never reaches the screen, no cart entry appears, and the dialog spins
        /// forever. Adding first lets the prompt show normally.
        /// </remarks>
        /// <returns><c>false</c> when the dialog could not be opened.</returns>
        public static async Task<bool> RunAsync(int productId,
            ClientServerCartViewService cartService,
            IClientDialogService dialogService)
        {
            if (dialogService is not ClientDialogService dialogs)
                return false;

            var before = cartService.ViewState.Products.Select(a => a.Guid).ToHashSet();

            cartService.AddProduct(productId);

            var entryId = await WaitForCartEntryAsync(cartService, before);

            //Declined the prompt, or the add failed: nothing to open the dialog on.
            if (entryId is null)
                return false;

            var dialog = await dialogs.ShowPackagePurchaseDialogAsync(productId, entryId.Value);

            if (dialog.Result == AddComponentResultCode.Opened)
                await dialog.WaitForResultAsync();

            return true;
        }

        /// <summary>
        /// Waits for a cart entry that was not there before.
        /// </summary>
        /// <remarks>
        /// Polling rather than a subscription: the cart raises a generic change, but what
        /// is awaited here is one specific entry. "Still reading the prompt" and "answered
        /// no" cannot be told apart - validation runs before the cart raises any busy flag -
        /// so the wait simply has a ceiling.
        /// </remarks>
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
