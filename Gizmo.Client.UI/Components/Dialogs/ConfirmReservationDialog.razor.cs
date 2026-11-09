using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ConfirmReservationDialog : CustomDOMComponentBase
    {
        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        [Inject]
        HostReservationViewState HostReservationViewState { get; set; }

        [Inject]
        ConfirmReservationDialogViewService ConfirmReservationDialogViewService { get; set; }

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<EmptyComponentResult> ResultCallback { get; set; }

        protected ConfirmReservationDialogViewState State => ConfirmReservationDialogViewService.ViewState;

        protected int Step => State.Step;

        protected bool CanIgnore => !HostReservationViewState.ReservationTimeReached && State.Step != 2;

        protected string DialogClass => State.Step == 2 ? "giz-tile-dialog giz-tile-dialog--success" : "giz-tile-dialog";

        protected string IconClass => State.Step switch
        {
            0 => "ph-bold ph-calendar-check",
            1 => "ph-bold ph-credit-card",
            _ => "ph-bold ph-check-circle"
        };

        protected string Title => GrafitLocalization.GetString(State.Step switch
        {
            0 => GrafitResourceKeys.SHELL_RESERVE_TITLE,
            1 => GrafitResourceKeys.SHELL_RESERVE_PAY_TITLE,
            _ => GrafitResourceKeys.SHELL_RESERVE_DONE
        });

        protected string CloseLabel => GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_CLOSE));

        protected bool HasError => !string.IsNullOrEmpty(State.ErrorMessage);

        protected bool IsConfirmDisabled => State.IsValid == false || State.IsLoading;

        protected bool HasPaymentMethods => State.AvailablePaymentMethods.Any();

        protected bool IsMethodLocked => State.IsLoading || State.HasQr;

        protected bool IsDepositMethod => State.SelectedPaymentMethod != null && State.PaymentMethodId == -3;

        protected bool IsBalanceShort => UserBalanceViewState.Balance < HostReservationViewState.Outstanding;

        protected bool IsPayDisabled => State.IsValid == false || State.IsLoading || State.HasQr || (IsDepositMethod && IsBalanceShort);

        protected bool HasReservationTime => HostReservationViewState.Time.HasValue && HostReservationViewState.Duration.HasValue;

        protected string ReservationTime => HasReservationTime
            ? $"{HostReservationViewState.Time.Value:g} – {HostReservationViewState.Time.Value.AddMinutes(HostReservationViewState.Duration.Value):t}"
            : string.Empty;

        protected string TotalText => Money(HostReservationViewState.Total.GetValueOrDefault());

        protected string PaidText => Money((HostReservationViewState.Total - HostReservationViewState.Outstanding).GetValueOrDefault());

        protected string OutstandingText => Money(HostReservationViewState.Outstanding.GetValueOrDefault());

        protected string BalanceText => Money(UserBalanceViewState.Balance);

        protected static string HostLabel(ReservationInfoHostModel host) => $"{host.HostNumber} · {host.HostName}";

        public Task<bool> SetInputPasswordCharacterIsValid(char value)
        {
            return Task.FromResult(char.IsNumber(value) && (string.IsNullOrEmpty(State.Pin) || State.Pin.Length <= 6));
        }

        private static string Money(decimal value) => value.ToString("C", CultureInfo.CurrentCulture);

        private void Ignore()
        {
            ConfirmReservationDialogViewService.Ignore();
        }

        private void Close()
        {
            ConfirmReservationDialogViewService.Close();
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
