using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenu : CustomDOMComponentBase
    {
        [Inject]
        public UserMenuViewService UserMenuViewService { get; set; }

        [Inject]
        public UserOnlineDepositViewState UserOnlineDepositViewState { get; set; }
    }
}
