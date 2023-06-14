using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

using static System.Net.Mime.MediaTypeNames;

namespace Gizmo.Client.UI.Components
{
    public partial class HtmlString : ComponentBase
    {
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Parameter] public string Content { get; set; }

        private MarkupString _htmlContent;

        private const string CommandPattern = @"(?<name>_\w+)=(?<quote>['""])(?<value>.*?)\k<quote>";
        private readonly HashSet<string> _supportedCommands = new()
        {
            "_onload"
        };

        protected override void OnInitialized() => _htmlContent = new(Content);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var regex = new Regex(CommandPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var matches = regex.Matches(Content);

            for (var i = 0; i < matches.Count; i++)
            {
                var command = matches[i].Groups;

                if (_supportedCommands.Contains(command["name"].Value))
                {
                    var value = command["value"].Value;

                    if (value[^2..] == "()")
                    {
                        value = value[..^2];
                    }

                    await JsRuntime.InvokeVoidAsync(value, value);
                }
            }
        }
    }
}
