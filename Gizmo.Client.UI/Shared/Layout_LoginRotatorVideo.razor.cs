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
        public EventCallback<VideoEventArgs> OnVideoEvent { get; set; }

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
                try
                {
                    VideoEventInterop = new VideoEventInterop(JsRuntime);
                    await VideoEventInterop.SetupVideoEventCallback(args => VideoHandler(args));
                    await JsRuntime.InvokeVoidAsync("registerVideoComponent", Ref);
                }
                catch (Exception ex)
                {

                }
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
