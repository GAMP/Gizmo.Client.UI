using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class HtmlString : ComponentBase
    {
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Parameter] public  string Content { get; set; }

        private MarkupString _htmlContent;
        private readonly HashSet<string> _htmlCommands = new()
        {
            "_onload"
        };

        protected override void OnInitialized() => _htmlContent = new(Content);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var regex = new Regex(@"(?<tag>\w+)='(?<function>.*?)'", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var commands = regex.Matches(Content);

            for (var i = 0; i < commands.Count; i++)
            {
                var command = commands[i];
                
                var tag = command.Groups["tag"].Value;
                var function = command.Groups["function"].Value;

                if (_htmlCommands.Contains(tag))
                {
                    await JsRuntime.InvokeVoidAsync(function, function);
                }
            }
        }
    }
}
