using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ClientTab : CustomDOMComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

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

        public override void Dispose()
        {
            _ = InvokeVoidAsync("unregisterTab", Ref);

            base.Dispose();
        }
    }
}