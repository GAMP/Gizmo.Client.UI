using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class UserAvatar : CustomDOMComponentBase
    {
        [Parameter]
        public string Picture { get; set; }

        protected bool HasPicture => !string.IsNullOrEmpty(Picture);
    }
}
