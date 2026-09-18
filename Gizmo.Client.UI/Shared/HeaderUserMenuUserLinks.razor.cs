using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenuUserLinks : CustomDOMComponentBase
    {
        [Inject]
        UserMenuViewState UserMenuViewState { get; set; }

        [Inject]
        UserViewState UserViewState { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserMenuViewState);
            this.SubscribeChange(UserViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserMenuViewState);
            this.UnsubscribeChange(UserViewState);

            base.Dispose();
        }
    }
}
