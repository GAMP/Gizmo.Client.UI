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
    public partial class TopBarReservation : CustomDOMComponentBase
    {
        #region FIELDS

        private static readonly TimeSpan COUNTDOWN_INTERVAL = TimeSpan.FromSeconds(30);
        private Timer? _countdownTick;

        #endregion

        [Inject]
        HostReservationViewState ViewState { get; set; } = null!;

        #region PROPERTIES

        /// <summary>
        /// Whether this machine has a reservation worth mentioning.
        /// </summary>
        /// <remarks>
        /// Ignored reservations are excluded - the shell already raises its own notification
        /// for those.
        /// </remarks>
        private bool HasReservation =>
            ViewState.ReservationId.HasValue
            && ViewState.Time.HasValue
            && !ViewState.Ignored
            && ViewState.Time.Value > DateTime.Now;

        /// <summary>
        /// True once the shell's own notification window has been reached. Drives a warmer
        /// colour rather than a second element.
        /// </summary>
        private bool IsClose => HasReservation && ViewState.ReservationNotificationTimeReached;

        private string StartText =>
            ViewState.Time?.ToString("HH:mm", CultureInfo.CurrentCulture) ?? string.Empty;

        /// <summary>
        /// Time left until the reservation starts, or null once it has started.
        /// </summary>
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

        /// <summary>
        /// Starts or stops the countdown tick to match what is on screen.
        /// </summary>
        /// <remarks>
        /// A countdown that silently freezes is worse than none, so it ticks while the tile
        /// is up and never otherwise: with no reservation there is nothing to redraw, and
        /// behind a game there is nobody reading it. The tick fires on a pool thread, hence
        /// <see cref="DispatchStateHasChanged"/>, which marshals to the renderer and
        /// absorbs teardown faults.
        /// </remarks>
        private void ApplyCountdown()
        {
            var wanted = ShellActivity.IsActive && HasReservation;

            if (wanted == (_countdownTick is not null))
                return;

            if (wanted)
            {
                _countdownTick = new Timer(_ => DispatchStateHasChanged(), null,
                    COUNTDOWN_INTERVAL, COUNTDOWN_INTERVAL);
            }
            else
            {
                _countdownTick?.Dispose();
                _countdownTick = null;
            }
        }

        //Static event arriving from JS interop; marshal to the UI thread.
        private void OnActivityChanged() => DispatchWorkflow(() =>
        {
            ApplyCountdown();
            return Task.CompletedTask;
        });

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            //Reservations appear and disappear on a server push, not on the tick, and a
            //render is the signal that something changed.
            ApplyCountdown();
        }

        public override void Dispose()
        {
            //Static event, outlives the component: unsubscribe before dropping the timer,
            //or the next focus change starts it again.
            ShellActivity.Changed -= OnActivityChanged;

            _countdownTick?.Dispose();
            _countdownTick = null;

            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        #endregion
    }
}
