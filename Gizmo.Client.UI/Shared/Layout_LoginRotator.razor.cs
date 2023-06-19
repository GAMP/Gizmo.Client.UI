using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class Layout_LoginRotator : CustomDOMComponentBase
    {
        [Inject]
        LoginRotatorViewState ViewState { get; set; }

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
