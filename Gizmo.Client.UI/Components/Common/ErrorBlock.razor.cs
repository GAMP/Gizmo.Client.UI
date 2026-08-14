using Gizmo.Web.Components;
using Gizmo.Web.Components.Extensions;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ErrorBlock : CustomDOMComponentBase
    {
        [Parameter]
        public string Message { get; set; }

        [Parameter]
        public bool ShowIcon { get; set; } = true;

        protected string ClassName => new ClassMapper()
            .Add("giz-error-block")
            .If("giz-error-block--with-icon", () => ShowIcon)
            .AsString();
    }
}
