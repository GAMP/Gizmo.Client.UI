using System;
using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Shared
{
    public partial class Layout_LoginRotatorVideo : CustomDOMComponentBase, IAsyncDisposable
    {
        [Parameter]
        public string MediaPath { get; set; }

        [Parameter]
        public string Preload { get; set; } = "auto";

        [Parameter]
        public double StartSeconds { get; set; }

        [Parameter]
        public EventCallback<VideoEventArgs> OnVideoEvent { get; set; }

        private string _appliedMediaPath;
        private double _appliedStartSeconds;

        private VideoEventInterop VideoEventInterop { get; set; }

        private async Task VideoHandler(VideoEventArgs args)
        {
            if (args.Id == Id)
            {
                await OnVideoEvent.InvokeAsync(args);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                _appliedMediaPath = MediaPath;
                _appliedStartSeconds = StartSeconds;

                try
                {
                    VideoEventInterop = new VideoEventInterop(JsRuntime);
                    await VideoEventInterop.SetupVideoEventCallback(args => VideoHandler(args));
                    await JsRuntime.InvokeVoidAsync("registerVideoComponent", Ref);
                }
                catch (Exception ex)
                {

                }

                return;
            }

            var sameMedia = string.Equals(_appliedMediaPath, MediaPath, StringComparison.OrdinalIgnoreCase);
            _appliedMediaPath = MediaPath;

            // A new file starts from canplay. The same file is seeked only when the clock moved it.
            if (!sameMedia)
            {
                _appliedStartSeconds = StartSeconds;
                return;
            }

            if (Math.Abs(_appliedStartSeconds - StartSeconds) < 0.5)
                return;

            _appliedStartSeconds = StartSeconds;

            try
            {
                await JsRuntime.InvokeVoidAsync("playVideo", Id, StartSeconds);
            }
            catch (Exception ex)
            {

            }
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterVideoComponent", Ref).ConfigureAwait(false);

            if (VideoEventInterop != null)
            {
                await VideoEventInterop.DisposeAsync();
            }

            Dispose();
        }

        #endregion
    }
}
