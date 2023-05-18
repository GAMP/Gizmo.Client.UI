using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    public partial class AppDetailsLink : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public AppLinkViewState Link { get; set; }

        #endregion

        protected override void OnInitialized()
        {
            if (Link != null)
            {
                this.SubscribeChange(Link);
            }

            base.OnInitialized();
        }

        public override void Dispose()
        {
            if (Link != null)
            {
                this.UnsubscribeChange(Link);
            }

            base.Dispose();
        }
    }
}
