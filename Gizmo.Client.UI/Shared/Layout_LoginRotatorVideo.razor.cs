using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Shared
{
    public partial class Layout_LoginRotatorVideo : CustomDOMComponentBase, IAsyncDisposable
    {
        [Inject]
        LoginRotatorViewService LoginRotatorViewService { get; set; }

        [Parameter]
        public string MediaPath { get; set; }

        private VideoEventInterop VideoEventInterop { get; set; }

        private async Task VideoHandler(VideoEventArgs args)
        {
            if (args.Id == Id)
            {
                if (args.VideoState == VideoStates.Ended)
                {
                    await LoginRotatorViewService.PlayNext();
                }
                else
                {
                    await JsRuntime.InvokeVoidAsync("playVideo", Ref);
                }
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
