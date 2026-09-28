using System.Globalization;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class LadderAvatarRing : CustomDOMComponentBase
    {
        [Inject]
        UserLadderSummaryViewState ViewState { get; set; }

        private string ProgressValue => ViewState.TopBarProgressPercent.ToString("0.##", CultureInfo.InvariantCulture);

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
