using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    /// <summary>
    /// Top bar status for application deployment (server → client file sync).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Where the numbers come from: every executable has an <c>AppExeExecutionViewState</c>
    /// whose <c>IsActive</c> is true while anything is being prepared for it, and whose
    /// <c>Progress</c>/<c>IsIndeterminate</c> are refreshed once a second by the services
    /// layer from the file syncer. There is no aggregate "is a deployment running" state
    /// anywhere, so this component builds one by scanning those per-executable states.
    /// </para>
    /// <para>
    /// Why polling and not subscriptions: progress updates are pushed into the individual
    /// view states, and the lookup service's own <c>Changed</c> event only fires when
    /// states are added/updated/removed - not when a tracked one ticks. Subscribing to
    /// every executable in the club (a hundred of them in a real venue) to catch that
    /// would cost more than a one second scan of an in-memory dictionary, which is also
    /// exactly the cadence the syncer publishes at.
    /// </para>
    /// </remarks>
    public partial class DeploymentBanner : CustomDOMComponentBase
    {
        /// <summary>
        /// How long a preparation has to run before it is worth interrupting the screen for.
        /// </summary>
        /// <remarks>
        /// Most launches finish their preparation step in well under a second - a banner
        /// for those would be a flash of noise on every single launch. Three seconds is
        /// the point where a customer starts wondering whether the click registered.
        /// </remarks>
        private static readonly TimeSpan ShowAfter = TimeSpan.FromSeconds(3);

        /// <summary>How long the finished state stays up before collapsing.</summary>
        private static readonly TimeSpan DoneLinger = TimeSpan.FromSeconds(5);

        private static readonly TimeSpan Tick = TimeSpan.FromSeconds(1);

        #region SERVICES

        [Inject] AppExeExecutionViewStateLookupService ExecutionStates { get; set; }

        [Inject] AppExeViewStateLookupService Executables { get; set; }

        [Inject] UserMenuViewService UserMenuViewService { get; set; }

        #endregion

        #region FIELDS

        private sealed class Track
        {
            public int ExeId;
            public string Caption = string.Empty;
            public bool HasDeploymentProfile;
            public DateTime StartedUtc;
            public DateTime? FinishedUtc;
            public bool WasShown;
            public decimal Progress;
            public bool Indeterminate;
        }

        private readonly Dictionary<int, Track> _tracks = new();
        private Timer _timer;
        private bool _scanning;

        //Everything below is what the markup reads. Recomputed on the tick, never in
        //the render pass, so a render never touches the clock and never disagrees with
        //itself between two lines of markup.
        private bool _open;
        private bool _done;
        private bool _indeterminate;
        private int _percent;
        private string _title = string.Empty;
        private string _subtitle = string.Empty;
        private string _signature = string.Empty;

        //Set of executables the customer has waved away; cleared once they all finish,
        //so the next launch gets a banner again.
        private readonly HashSet<int> _dismissed = new();

        #endregion

        #region PROPERTIES

        protected bool IsOpen => _open;
        protected bool IsDone => _done;
        protected bool IsIndeterminate => _indeterminate;
        protected int PercentValue => _percent;
        protected string PercentLabel => _indeterminate ? "…" : $"{_percent}%";
        protected string Title => _title;
        protected string Subtitle => _subtitle;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            _timer = new Timer(OnTick, null, Tick, Tick);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            _timer = null;

            //Never leave the slot marked as taken by a component that is going away.
            TopBannerArbiter.SetDeploymentVisible(false);

            base.Dispose();
        }

        #endregion

        #region EVENTS

        //Timer callbacks arrive on a pool thread. This must not be async void: an
        //exception from one would come back on the pool and take the whole client
        //down with it (see CustomComponentBase.DispatchWorkflow).
        private void OnTick(object _)
        {
            if (_scanning)
                return;

            _scanning = true;

            DispatchWorkflow(async () =>
            {
                try
                {
                    await ScanAsync();
                }
                finally
                {
                    _scanning = false;
                }
            });
        }

        private void OpenManager()
        {
            UserMenuViewService.ToggleActiveApps();
        }

        private void Dismiss()
        {
            foreach (var track in _tracks.Values.Where(a => a.FinishedUtc == null))
                _dismissed.Add(track.ExeId);

            //A finished banner has nothing left to dismiss but itself.
            foreach (var track in _tracks.Values.Where(a => a.FinishedUtc != null))
                track.WasShown = false;

            Recompute();
        }

        #endregion

        #region HELPERS

        private async Task ScanAsync()
        {
            IEnumerable<Gizmo.Client.UI.View.States.AppExeExecutionViewState> states;

            try
            {
                states = await ExecutionStates.GetStatesAsync();
            }
            catch
            {
                //Before login there is nothing to ask about and the lookup can refuse
                //outright. A status banner is not worth a log line a second for it.
                return;
            }

            var now = DateTime.UtcNow;

            var activeIds = new HashSet<int>();

            foreach (var state in states)
            {
                if (!state.IsActive)
                    continue;

                activeIds.Add(state.AppExeId);

                if (!_tracks.TryGetValue(state.AppExeId, out var track))
                {
                    track = new Track { ExeId = state.AppExeId, StartedUtc = now };

                    //Caption and "does this executable even have a deployment profile"
                    //come from a different lookup; resolved once, when first seen.
                    var exe = await Executables.GetStateAsync(state.AppExeId);
                    if (exe != null)
                    {
                        track.Caption = exe.Caption ?? string.Empty;
                        track.HasDeploymentProfile = exe.DeploymentProfiles?.Any() == true;
                    }

                    _tracks[state.AppExeId] = track;
                }

                track.FinishedUtc = null;
                track.Progress = state.Progress;
                track.Indeterminate = state.IsIndeterminate;
            }

            foreach (var track in _tracks.Values)
            {
                if (activeIds.Contains(track.ExeId))
                    continue;

                track.FinishedUtc ??= now;
            }

            //Drop everything that has been finished long enough to be off screen, and
            //let a customer who waved this one away be nudged again next time.
            foreach (var stale in _tracks.Values
                         .Where(a => a.FinishedUtc.HasValue && now - a.FinishedUtc.Value > DoneLinger)
                         .Select(a => a.ExeId)
                         .ToList())
            {
                _tracks.Remove(stale);
                _dismissed.Remove(stale);
            }

            Recompute();
        }

        private void Recompute()
        {
            var now = DateTime.UtcNow;

            //Live = running long enough to be worth showing and not waved away.
            var live = _tracks.Values
                .Where(a => a.FinishedUtc == null
                            && now - a.StartedUtc >= ShowAfter
                            && !_dismissed.Contains(a.ExeId))
                .ToList();

            foreach (var track in live)
                track.WasShown = true;

            //Finished ones only get a closing word if their progress was actually on
            //screen; a deployment that took two seconds says nothing at all.
            var justDone = _tracks.Values
                .Where(a => a.FinishedUtc.HasValue
                            && a.WasShown
                            && now - a.FinishedUtc.Value <= DoneLinger)
                .ToList();

            _open = live.Count > 0 || justDone.Count > 0;
            _done = live.Count == 0 && justDone.Count > 0;

            if (_done)
            {
                _title = "Готово";
                _subtitle = justDone.Count == 1
                    ? $"Запускаем: {Shorten(justDone[0].Caption)}"
                    : $"{justDone.Count} {Plural(justDone.Count, "приложение", "приложения", "приложений")} готовы";
                _indeterminate = false;
                _percent = 100;
            }
            else if (live.Count > 0)
            {
                //"Развёртывание" only when there is actually a deployment profile behind
                //it; anything else preparing itself is honestly just "Подготовка".
                _title = live.Any(a => a.HasDeploymentProfile) ? "Развёртывание" : "Подготовка";

                var measurable = live.Where(a => !a.Indeterminate).ToList();

                //Indeterminate only while nothing at all can be measured. As soon as one
                //file sync reports totals, its number carries the whole banner - a bar
                //that keeps flipping between "…" and a percentage reads as broken.
                _indeterminate = measurable.Count == 0;
                _percent = measurable.Count == 0
                    ? 0
                    : (int)Math.Round(measurable.Average(a => a.Progress));

                if (live.Count == 1)
                {
                    _subtitle = string.IsNullOrWhiteSpace(live[0].Caption)
                        ? "Копируем файлы игры"
                        : Shorten(live[0].Caption);
                }
                else
                {
                    //Длиннее не влезает в пилюлю — остальное за кликом.
                    _subtitle = $"{live.Count} {Plural(live.Count, "приложение", "приложения", "приложений")} · открыть список";
                }
            }

            var signature = $"{_open}|{_done}|{_indeterminate}|{_percent}|{_title}|{_subtitle}";
            if (signature == _signature)
                return;

            _signature = signature;

            TopBannerArbiter.SetDeploymentVisible(_open);
            DispatchStateHasChanged();
        }

        private static string Shorten(string value) =>
            string.IsNullOrEmpty(value) || value.Length <= 34 ? value : value[..33].TrimEnd() + "…";

        private static string Plural(int count, string one, string few, string many)
        {
            var mod100 = count % 100;
            if (mod100 is >= 11 and <= 14)
                return many;

            return (count % 10) switch
            {
                1 => one,
                2 or 3 or 4 => few,
                _ => many,
            };
        }

        #endregion
    }
}
