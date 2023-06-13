using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class HtmlString : ComponentBase
    {
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Parameter] public  string Content { get; set; }

        private MarkupString _html;

        protected override async Task OnInitializedAsync()
        {
            _html = await ParseHtmlString(Content);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Content.Contains("onload"))
            { 
                await JsRuntime.InvokeVoidAsync("ExternalFunctions.Advertisement.OnLoad", "Advertisement");
            }
        }

        private async Task<MarkupString> ParseHtmlString(string row) 
        {
            return new MarkupString(row);
        }
    }
}
