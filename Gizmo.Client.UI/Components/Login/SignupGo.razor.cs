using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class SignupGo : CustomDOMComponentBase
    {
        [Parameter]
        public string Label { get; set; }

        [Parameter]
        public bool IsBusy { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsFinal { get; set; }

        [Parameter]
        public EventCallback OnClick { get; set; }

        protected string IconClass => IsFinal ? "ph-bold ph-check" : "ph-bold ph-arrow-right";
    }
}
