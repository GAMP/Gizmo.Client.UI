using Gizmo.UI.View.States;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class NotificationsHost : ComponentBase
    {
        [Inject()]
        private NotificationsHostViewState ViewState
        {
            get;set;
        }

        private void CloseNotifications()
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.SubscribeChange(ViewState);
        }
    }
}
