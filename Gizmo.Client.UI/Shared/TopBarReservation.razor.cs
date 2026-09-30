using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class TopBarReservation : ShellComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        #region FIELDS

        private static readonly TimeSpan COUNTDOWN_INTERVAL = TimeSpan.FromSeconds(30);
        private Timer? _countdownTick;

        #endregion

        [Inject]
        HostReservationViewService ReservationService { get; set; } = null!;

        private HostReservationViewState ViewState => ReservationService.ViewState;

        #region PROPERTIES

        private bool HasReservation =>
            ViewState.ReservationId.HasValue
            && ViewState.Time.HasValue
            && ViewState.Time.Value > DateTime.Now;

        private bool IsIgnored => HasReservation && ViewState.Ignored;

        private bool IsClose => HasReservation && ViewState.ReservationNotificationTimeReached;

        private string ClassName => (IsClose, IsIgnored) switch
        {
            (true, true) => "giz-resv giz-resv--soon giz-resv--ignored",
            (true, false) => "giz-resv giz-resv--soon",
            (false, true) => "giz-resv giz-resv--ignored",
            _ => "giz-resv"
        };

        private string LeadText => GrafitLocalization.GetString(IsClose
            ? GrafitResourceKeys.SHELL_RESV_SHORT
            : GrafitResourceKeys.SHELL_RESV_DEVICE_RESERVED);

        private string FromText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_RESV_FROM,
            ViewState.Time?.ToString("t", CultureInfo.CurrentCulture) ?? string.Empty);

        private Task OpenReservation() =>
            IsIgnored ? ReservationService.ShowDialog() : Task.CompletedTask;

        private Task OnReservationKey(KeyboardEventArgs args) =>
            args.Key is "Enter" or " " ? OpenReservation() : Task.CompletedTask;

        private string? CountdownText
        {
            get
            {
                if (!HasReservation)
                    return null;

                var left = ViewState.Time!.Value - DateTime.Now;

                return left > TimeSpan.Zero
                    ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_RESV_IN, FormatSpan(left))
                    : null;
            }
        }

        private string FormatSpan(TimeSpan span)
        {
            var hours = (int)span.TotalHours;
            var minutes = span.Minutes;

            if (hours > 0)
                return minutes > 0
                    ? GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DURATION_HOURS_MINUTES, hours, minutes)
                    : GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DURATION_HOURS, hours);

            return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_DURATION_MINUTES, Math.Max(minutes, 1));
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            ShellActivity.Changed += OnActivityChanged;

            ApplyCountdown();

            base.OnInitialized();
        }

        private void ApplyCountdown()
        {
            var wanted = ShellActivity.IsActive && HasReservation;

            if (wanted == (_countdownTick is not null))
                return;

            if (wanted)
            {
                _countdownTick = new Timer(_ => DispatchRender(), null,
                    COUNTDOWN_INTERVAL, COUNTDOWN_INTERVAL);
            }
            else
            {
                _countdownTick?.Dispose();
                _countdownTick = null;
            }
        }

        private void OnActivityChanged() => DispatchWorkflow(() =>
        {
            ApplyCountdown();
            return Task.CompletedTask;
        });

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            ApplyCountdown();
        }

        public override void Dispose()
        {
            ShellActivity.Changed -= OnActivityChanged;

            _countdownTick?.Dispose();
            _countdownTick = null;

            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        #endregion
    }
}
