using System;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class SearchMatch : CustomDOMComponentBase
    {
        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public string Pattern { get; set; }

        private string Needle => Pattern?.Trim() ?? string.Empty;

        private int HitIndex => string.IsNullOrEmpty(Text) || Needle.Length == 0
            ? -1
            : Text.IndexOf(Needle, StringComparison.OrdinalIgnoreCase);

        private string Before => Text[..HitIndex];

        private string Hit => Text.Substring(HitIndex, Needle.Length);

        private string After => Text[(HitIndex + Needle.Length)..];
    }
}
