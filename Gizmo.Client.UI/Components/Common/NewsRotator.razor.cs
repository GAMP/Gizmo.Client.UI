using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class NewsRotator : CustomDOMComponentBase
    {
        [Inject]
        public FeedsViewService FeedsViewService { get; set; }

        [Inject]
        public FeedsViewState ViewState { get; set; }

        private Task OnMouseOverHandler(MouseEventArgs args)
        {
            return FeedsViewService.PauseAsync(true);
        }

        private Task OnMouseOutHandler(MouseEventArgs args)
        {
            return FeedsViewService.PauseAsync(false);
        }

        #region OVERRIDES

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

        #endregion
    }
}
