using System;
using System.Threading.Tasks;

using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class GizScroller : CustomDOMComponentBase, IAsyncDisposable
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected string ClassName => string.IsNullOrEmpty(Class) ? "giz-scroller" : "giz-scroller " + Class;

        private Task Back() => InvokeVoidAsync("grafitScroller.step", Ref, -1).AsTask();

        private Task Next() => InvokeVoidAsync("grafitScroller.step", Ref, 1).AsTask();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
                await InvokeVoidAsync("grafitScroller.attach", Ref);
            else
                await InvokeVoidAsync("grafitScroller.reveal", Ref, false);
        }

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("grafitScroller.detach", Ref).ConfigureAwait(false);

            Dispose();
        }
    }
}
