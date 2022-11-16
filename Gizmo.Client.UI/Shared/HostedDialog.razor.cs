using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class HostedDialog : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        #endregion
    }
}
