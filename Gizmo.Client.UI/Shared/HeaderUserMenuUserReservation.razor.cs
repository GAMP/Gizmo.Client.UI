using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserMenuUserReservation : CustomDOMComponentBase
    {
        [Inject]
        public HostReservationViewService HostReservationViewService { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(HostReservationViewService.ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(HostReservationViewService.ViewState);

            base.Dispose();
        }
    }
}
