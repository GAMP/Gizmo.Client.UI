using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class UserLock
    {
        [Inject]
        UserService UserService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserService.ViewState);
            base.OnInitialized();
        }
    }
}
