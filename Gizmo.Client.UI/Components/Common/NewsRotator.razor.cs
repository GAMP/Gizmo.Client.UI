using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class NewsRotator : CustomDOMComponentBase
    {
        [Inject]
        public FeedsViewState FeedsViewState { get; set; }

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(FeedsViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(FeedsViewState);

            base.Dispose();
        }

        #endregion
    }
}
