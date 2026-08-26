using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Threading;

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
        /// Ignored reservations are excluded: the shell already raises its own
        /// notification for those, and repeating it in the top bar would turn a
        /// quiet statement of fact into a second nag.
        /// </remarks>
        private bool HasReservation =>
            ViewState.ReservationId.HasValue
            && ViewState.Time.HasValue
            && !ViewState.Ignored
            && ViewState.Time.Value > DateTime.Now;

        /// <summary>
        /// True once the shell's own notification window has been reached.
        /// </summary>
        /// <remarks>
        /// Drives a warmer colour rather than a second element - same
        /// information, more weight.
        /// </remarks>
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
                return minutes > 0 ? $"{hours} ч {minutes} мин" : $"{hours} ч";

            return $"{Math.Max(minutes, 1)} мин";
        }

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            //A countdown that silently freezes is worse than none, because it
            //still reads as current. The tick fires on a pool thread, so it goes
            //through DispatchStateHasChanged, which marshals to the renderer and
            //swallows teardown faults - a raw StateHasChanged here would be the
            //same class of crash that took the client down before.
            _countdownTick = new Timer(_ => DispatchStateHasChanged(), null,
                COUNTDOWN_INTERVAL, COUNTDOWN_INTERVAL);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            _countdownTick?.Dispose();
            _countdownTick = null;

            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }

        #endregion
    }
}
