using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ClientTab : CustomDOMComponentBase, IAsyncDisposable
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TabControlsPositions ControlsPosition { get; set; } = TabControlsPositions.External;

        //Returning the task rather than being async void lets Blazor own any JS interop failure
        //(a scroll against a WebView that is going away). As async void it would instead be
        //rethrown on the thread pool and exit the whole client.
        private Task OnClickPreviousButton()
        {
            return InvokeVoidAsync("tabScrollPrevious", Ref).AsTask();
        }

        private Task OnClickNextButton()
        {
            return InvokeVoidAsync("tabScrollNext", Ref).AsTask();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InvokeVoidAsync("registerTab", Ref);
            }
        }

        #region CLASSMAPPERS

        protected string ClassName => new ClassMapper()
                 .Add("giz-client-tab")
                 .If("giz-client-tab--internal", () => ControlsPosition == TabControlsPositions.Internal)
                 .AsString();

        #endregion

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterTab", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}