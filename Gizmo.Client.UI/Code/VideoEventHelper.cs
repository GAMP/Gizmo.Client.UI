using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI
{
    public enum VideoStates
    {
        CanPlayThrough = 0,
        Ended = 1
    }

    public class VideoEventArgs : EventArgs
    {
        public string Id { get; set; }
        public VideoStates VideoState { get; set; }
    }

    public class VideoEventHelper
    {
        private readonly Func<VideoEventArgs, Task> _callback;

        public VideoEventHelper(Func<VideoEventArgs, Task> callback)
        {
            _callback = callback;
        }

        [JSInvokable]
        public Task OnVideoEvent(VideoEventArgs args) => _callback(args);
    }

    public class VideoEventInterop : IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<VideoEventHelper> Reference;

        public VideoEventInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> SetupVideoEventCallback(Func<VideoEventArgs, Task> callback)
        {
            Reference = DotNetObjectReference.Create(new VideoEventHelper(callback));
            return _jsRuntime.InvokeAsync<string>("addVideoEventListener", Reference);
        }

        public async ValueTask DisposeAsync() => await _jsRuntime.InvokeAsync<string>("removeVideoEventListener", Reference);
    }
}
