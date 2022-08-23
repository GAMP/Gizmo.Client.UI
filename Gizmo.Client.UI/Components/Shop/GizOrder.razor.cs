using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class GizOrder : CustomDOMComponentBase
    {
        [Inject]
        UserCartService UserCartService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserCartService.ViewState);
            base.OnInitialized();
        }
    }
}
