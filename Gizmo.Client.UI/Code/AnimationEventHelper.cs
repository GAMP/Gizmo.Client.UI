using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI
{
    public enum AnimationStates
    {
        Start = 0,
        Iteration = 1,
        End = 2,
        Cancel = 3
    }

    public class AnimationEventArgs : EventArgs
    {
        public string Id { get; set; }
        public AnimationStates AnimationState { get; set; }
        public string AnimationName { get; set; }
    }

    public class AnimationEventHelper
    {
        private readonly Func<AnimationEventArgs, Task> _callback;

        public AnimationEventHelper(Func<AnimationEventArgs, Task> callback)
        {
            _callback = callback;
        }

        [JSInvokable]
        public Task OnAnimationEvent(AnimationEventArgs args) => _callback(args);
    }

    public class AnimationEventInterop : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<AnimationEventHelper> Reference;

        public AnimationEventInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> SetupAnimationEventCallback(Func<AnimationEventArgs, Task> callback)
        {
            Reference = DotNetObjectReference.Create(new AnimationEventHelper(callback));
            return _jsRuntime.InvokeAsync<string>("addAnimationEventListener", Reference);
        }

        public async ValueTask DisposeAsync() => await _jsRuntime.InvokeAsync<string>("removeAnimationEventListener", Reference);
    }
}
