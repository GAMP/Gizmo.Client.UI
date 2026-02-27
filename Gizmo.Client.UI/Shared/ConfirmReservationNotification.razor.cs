using System.Threading.Tasks;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class ConfirmReservationNotification : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ConfirmReservationNotificationViewService ConfirmReservationNotificationViewService { get; set; }

        [CascadingParameter]
        protected NotificationsHost Parent { get; set; }

        [Parameter]
        public int Identifier { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<EmptyComponentResult> ResultCallback { get; set; }

        [Parameter]
        public EventCallback<int> OnClose { get; set; }

        private Task OpenPaymentDialogAsync()
        {
            return ConfirmReservationNotificationViewService.OpenPaymentDialogAsync();
        }

        private void Ignore()
        {
            ConfirmReservationNotificationViewService.Ignore();
        }

        private async Task CloseNotification()
        {
            ConfirmReservationNotificationViewService.Dismiss();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ConfirmReservationNotificationViewService.ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ConfirmReservationNotificationViewService.ViewState);

            base.Dispose();
        }
    }
}
