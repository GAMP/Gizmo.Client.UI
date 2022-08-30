using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class UserLock
    {
        [Inject]
        UserLockService UserLockService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserLockService.ViewState);
            base.OnInitialized();
        }
    }
}
