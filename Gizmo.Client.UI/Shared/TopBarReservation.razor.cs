using Gizmo.Client.UI.Localization;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class TopBarReservation : ShellComponentBase
    {
        #region FIELDS

        private static readonly TimeSpan COUNTDOWN_INTERVAL = TimeSpan.FromSeconds(30);
        private Timer? _countdownTick;

        #endregion

        [Inject]
        HostReservationViewState ViewState { get; set; } = null!;

        #region PROPERTIES

        private bool HasReservation =>
            ViewState.ReservationId.HasValue
            && ViewState.Time.HasValue
            && !ViewState.Ignored
            && ViewState.Time.Value > DateTime.Now;

        private bool IsClose => HasReservation && ViewState.ReservationNotificationTimeReached;

        private string StartText =>
            ViewState.Time?.ToString("HH:mm", CultureInfo.CurrentCulture) ?? string.Empty;

        private string? CountdownText
        {
            get
            {
                if (!HasReservation)
                    return null;

                var left = ViewState.Time!.Value - DateTime.Now;

                return left > TimeSpan.Zero ? FormatSpan(left) : null;
            }
        }

        private static string FormatSpan(TimeSpan span)
        {
            var hours = (int)span.TotalHours;
            var minutes = span.Minutes;

            if (hours > 0)
                return minutes > 0
                    ? ShellStringOverrides.Get(ShellStringOverrides.DURATION_HOURS_MINUTES, hours, minutes)
                    : ShellStringOverrides.Get(ShellStringOverrides.DURATION_HOURS, hours);

            return ShellStringOverrides.Get(ShellStringOverrides.DURATION_MINUTES, Math.Max(minutes, 1));
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
