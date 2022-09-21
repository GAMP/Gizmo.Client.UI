using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class UserLock : CustomDOMComponentBase
    {
        [Inject]
        UserLockService UserLockService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserLockService.ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserLockService.ViewState);
            base.Dispose();
        }
    }
}
