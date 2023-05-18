using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Components
{
    public partial class QuickLauncherQuickLaunch : CustomDOMComponentBase
    {
        [Inject]
        IOptions<PopularItemsOptions> PopularItemsOptions { get; set; }

        [Inject]
        QuickLaunchViewState ViewState { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
