using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProfileHeader : CustomDOMComponentBase
    {
        public ProfileHeader()
        {
        }

        [Inject]
        UserService UserService { get; set; }
    }
}
