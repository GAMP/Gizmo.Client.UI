using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI
{
    public class WindowMouseDownEventHelper
    {
        private readonly Func<MouseEventArgs, Task> _callback;

        public WindowMouseDownEventHelper(Func<MouseEventArgs, Task> callback)
        {
            _callback = callback;
        }

        [JSInvokable]
        public Task OnWindowMouseDownEvent(MouseEventArgs args) => _callback(args);
    }

    public class WindowMouseDownEventInterop : IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<WindowMouseDownEventHelper> Reference;

        public WindowMouseDownEventInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> SetupWindowMouseDownEventCallback(Func<MouseEventArgs, Task> callback)
        {
            Reference = DotNetObjectReference.Create(new WindowMouseDownEventHelper(callback));
            return _jsRuntime.InvokeAsync<string>("addWindowMouseDownEventListener", Reference);
        }

        public void Dispose()
        {
            _jsRuntime.InvokeAsync<string>("removeWindowMouseDownEventListener", Reference);
            //Reference?.Dispose();
        }
    }
}
