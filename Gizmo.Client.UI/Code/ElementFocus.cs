using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI
{
    public static class ElementFocus
    {
        public static async Task TryAsync(Func<ValueTask> focus)
        {
            try
            {
                await focus();
            }
            catch (Exception exception) when (exception is JSException or InvalidOperationException or TaskCanceledException or ObjectDisposedException)
            {
                // The element left the page between the render and the focus: nothing to focus.
            }
        }
    }
}
