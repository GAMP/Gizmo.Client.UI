using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public sealed record CodeBox(string Char, string ClassName);

    public partial class CodeBoxes : ShellComponentBase
    {
        private const int FALLBACK_LENGTH = 6;
        private const int LONGEST_CODE = 12;

        private ElementReference _input;

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public int Length { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool HasError { get; set; }

        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        private string Text => Value ?? string.Empty;

        private bool KnowsLength => Length > 0 && Length <= LONGEST_CODE;

        protected int? MaxLength => KnowsLength ? Length : null;

        protected string ClassName => HasError ? "giz-code-boxes giz-code-boxes--error" : "giz-code-boxes";

        protected IReadOnlyList<CodeBox> Boxes
        {
            get
            {
                var text = Text;
                var count = KnowsLength ? Length : Math.Max(FALLBACK_LENGTH, text.Length + 1);
                var boxes = new List<CodeBox>(count);

                for (var i = 0; i < count; i++)
                {
                    if (i < text.Length)
                        boxes.Add(new CodeBox(text[i].ToString(), "giz-code-boxes__box giz-code-boxes__box--filled"));
                    else if (i == text.Length)
                        boxes.Add(new CodeBox(string.Empty, "giz-code-boxes__box giz-code-boxes__box--empty giz-code-boxes__box--next"));
                    else
                        boxes.Add(new CodeBox(string.Empty, "giz-code-boxes__box giz-code-boxes__box--empty"));
                }

                return boxes;
            }
        }

        protected async Task OnInputAsync(ChangeEventArgs args)
        {
            var value = (args.Value as string ?? string.Empty).Trim();

            if (KnowsLength && value.Length > Length)
                value = value[..Length];

            await ValueChanged.InvokeAsync(value);

            if (KnowsLength && value.Length == Length)
                await OnSubmit.InvokeAsync();
        }

        protected Task OnKeyDownAsync(KeyboardEventArgs args)
        {
            return args.Key == "Enter" ? OnSubmit.InvokeAsync() : Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await ElementFocus.TryAsync(() => _input.FocusAsync());

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
