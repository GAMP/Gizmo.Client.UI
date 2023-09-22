using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenuNotifications : CustomDOMComponentBase
    {
        [Inject]
        NotificationsHostViewState ViewState { get; set; }

        [Inject]
        UserMenuViewState UserMenuViewState { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserMenuViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserMenuViewState);
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
