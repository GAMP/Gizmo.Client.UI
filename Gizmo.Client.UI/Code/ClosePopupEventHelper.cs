using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI
{
    public class ClosePopupEventHelper
    {
        private readonly Func<string, Task> _callback;

        public ClosePopupEventHelper(Func<string, Task> callback)
        {
            _callback = callback;
        }

        [JSInvokable]
        public Task OnClosePopupEvent(string args) => _callback(args);
    }

    public class ClosePopupEventInterop : IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<ClosePopupEventHelper> Reference;

        public ClosePopupEventInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> SetupClosePopupEventCallback(Func<string, Task> callback)
        {
            Reference = DotNetObjectReference.Create(new ClosePopupEventHelper(callback));
            return _jsRuntime.InvokeAsync<string>("addClosePopupEventListener", Reference);
        }

        public void Dispose()
        {
            _jsRuntime.InvokeAsync<string>("removeClosePopupEventListener", Reference);
            //Reference?.Dispose();
        }
    }
}