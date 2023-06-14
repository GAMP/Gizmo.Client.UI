using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class HtmlString : ComponentBase
    {
        [Inject] IJSRuntime JsRuntime { get; set; }
        [Inject] ILogger<HtmlString> Logger { get; set; }

        [Parameter] public string Content { get; set; }
        [Parameter] public Dictionary<string, object> ContentParameters { get; set; } = new();

        private MarkupString _htmlContent;

        private const string CommandPattern = @"(?<name>_\w+)=(?<quote>['""])(?<value>.*?)\k<quote>";
        private readonly HashSet<string> _supportedCommands = new()
        {
            "_onload"
        };

        protected override void OnInitialized() => _htmlContent = new(Content);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var regex = new Regex(CommandPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                var matches = regex.Matches(Content);

                for (var i = 0; i < matches.Count; i++)
                {
                    var command = matches[i].Groups;
                    var commandName = command["name"].Value;
                    var commandValue = command["value"].Value;

                    if (_supportedCommands.Contains(commandName))
                    {
                        try
                        {
                            var functionParametersValue = GetFunctionParametersValue(commandValue.AsSpan());

                            if (!string.IsNullOrWhiteSpace(functionParametersValue))
                            {
                                //Add the function parameters to the Client.UI parameters
                                ContentParameters.Add("Data", functionParametersValue);
                            }
                            
                            var functionName = GetFunctionName(commandValue.AsSpan());

                            await JsRuntime.InvokeVoidAsync(functionName, ContentParameters);
                        }
                        catch (Exception exeption)
                        {
                            Logger.LogError(exeption, "Error parsing for the costom event: {0}={1}", commandName, commandValue);
                        }

                        //Stop parsing after the first supported command
                        break;
                    }
                }
            }
        }

        private static string GetFunctionName(ReadOnlySpan<char> commandValue)
        {
            var index = commandValue.IndexOf('(');

            return index == -1
                ? commandValue.ToString()
                : commandValue[..index].ToString();
        }

        private static string GetFunctionParametersValue(ReadOnlySpan<char> commandValue)
        {
            var startIndex = commandValue.IndexOf('(') + 1;
            var endIndex = commandValue.LastIndexOf(')');

            return startIndex < endIndex
                ? commandValue[startIndex..endIndex].TrimEnd(new char[] { '\'', '"' }).ToString()
                : string.Empty;
        }
    }
}
