using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI
{
    public class ExpansionPanelEventArgs : EventArgs
    {
        public string Id { get; set; }
        public bool IsCollapsed { get; set; }
    }

    public class ExpansionPanelEventHelper
    {
        private readonly Func<ExpansionPanelEventArgs, Task> _callback;

        public ExpansionPanelEventHelper(Func<ExpansionPanelEventArgs, Task> callback)
        {
            _callback = callback;
        }

        [JSInvokable]
        public Task OnExpansionPanelEvent(ExpansionPanelEventArgs args) => _callback(args);
    }

    public class ExpansionPanelEventInterop : IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<ExpansionPanelEventHelper> Reference;

        public ExpansionPanelEventInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> SetupExpansionPanelEventCallback(Func<ExpansionPanelEventArgs, Task> callback)
        {
            Reference = DotNetObjectReference.Create(new ExpansionPanelEventHelper(callback));
            return _jsRuntime.InvokeAsync<string>("addExpansionPanelEventListener", Reference);
        }

        public void Dispose()
        {
            _jsRuntime.InvokeAsync<string>("removeExpansionPanelEventListener", Reference);
            //Reference?.Dispose();
        }
    }
}
