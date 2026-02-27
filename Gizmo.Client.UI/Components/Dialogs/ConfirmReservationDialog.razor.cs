using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ConfirmReservationDialog : CustomDOMComponentBase
    {
        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        HostReservationViewState HostReservationViewState { get; set; }

        [Inject]
        ConfirmReservationDialogViewService ConfirmReservationDialogViewService { get; set; }

        [Inject]
        IGizmoClient _gizmoClient { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<EmptyComponentResult> ResultCallback { get; set; }

        public Task<bool> SetInputPasswordCharacterIsValid(char value)
        {
            return Task.FromResult(char.IsNumber(value) && (string.IsNullOrEmpty(ConfirmReservationDialogViewService.ViewState.Pin) || ConfirmReservationDialogViewService.ViewState.Pin.Length <= 6));
        }

        private void Ignore()
        {
            ConfirmReservationDialogViewService.Ignore();
        }

        private void Close()
        {
            ConfirmReservationDialogViewService.Close();
        }

        private string GetReservationTime()
        {
            if (HostReservationViewState.Time.HasValue && HostReservationViewState.Duration.HasValue)
                return $"{HostReservationViewState.Time.Value} - {HostReservationViewState.Time.Value.AddMinutes(HostReservationViewState.Duration.Value)}";

            return string.Empty;
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(HostReservationViewState);
            this.SubscribeChange(ConfirmReservationDialogViewService.ViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ConfirmReservationDialogViewService.ViewState);
            this.UnsubscribeChange(HostReservationViewState);

            base.Dispose();
        }
    }
}
