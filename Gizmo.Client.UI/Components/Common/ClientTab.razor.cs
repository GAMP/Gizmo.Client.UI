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

        private async void OnClickPreviousButton()
        {
            await InvokeVoidAsync("tabScrollPrevious", Ref);
        }

        private async void OnClickNextButton()
        {
            await InvokeVoidAsync("tabScrollNext", Ref);
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