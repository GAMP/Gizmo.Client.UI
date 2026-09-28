using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class TimeAmount : ComponentBase
    {
        [Parameter] public string Value { get; set; }

        protected bool IsUnlimited => string.IsNullOrWhiteSpace(Value);
    }
}
