using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    public partial class AppDetailsMediaItem : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Parameter]
        public AppLinkViewState Link { get; set; }

        #endregion

        protected override void OnInitialized()
        {
            this.SubscribeChange(Link);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Link);

            base.Dispose();
        }
    }
}
