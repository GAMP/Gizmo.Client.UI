using Gizmo.Client.UI.Localization;
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
    /// Top bar status for application deployment (server to client file sync).
    /// </summary>
    /// <remarks>
    /// Every executable has an <c>AppExeExecutionViewState</c> whose <c>IsActive</c> is
    /// true while anything is being prepared for it, with <c>Progress</c> refreshed once a
    /// second from the file syncer. There is no aggregate "a deployment is running" state,
    /// so this component builds one by scanning those.
    /// <para>
    /// Polling rather than subscriptions: the lookup's <c>Changed</c> event fires when
    /// states are added, updated or removed, not when a tracked one ticks. Subscribing to
    /// every executable in a venue would cost more than a one second scan of an in-memory
    /// dictionary, which is also the cadence the syncer publishes at.
    /// </para>
    /// </remarks>
    public partial class DeploymentBanner : ShellComponentBase
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

        //First scan after focus returns: an executable that is no longer active did not
        //just finish, it finished while the shell was in the background.
        private bool _dropMissingOnce;

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

        /// <summary>
        /// Dash offset of the progress ring: the circumference of the r=17.5 circle in the
        /// markup, less the part that is done. Zero (a closed ring) while indeterminate -
        /// the stylesheet shortens the dash and turns it then - and when done.
        /// </summary>
        protected string RingOffset
        {
            get
            {
                const double circumference = 2 * Math.PI * 17.5;
                var done = _done || _indeterminate ? 1.0 : Math.Clamp(_percent, 0, 100) / 100.0;
                return (circumference * (1 - done)).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        protected string Title => _title;
        protected string Subtitle => _subtitle;

        #endregion

        #region OVERRIDES

        protected override void OnInitialized()
        {
            ShellActivity.Changed += OnActivityChanged;

            ApplyTimer();

            base.OnInitialized();
        }

        /// <summary>
        /// Runs the scan only while somebody can see its result.
        /// </summary>
        /// <remarks>
        /// A second is the right cadence for a progress bar being watched and pure waste
        /// behind a running game, where the host does not suspend the WebView and every
        /// render costs the game a frame. On the first tick after focus returns the banner
        /// is correct again.
        /// </remarks>
        private void ApplyTimer()
        {
            var wanted = ShellActivity.IsActive;

            if (wanted == (_timer is not null))
                return;

            if (wanted)
            {
                //Anything that finished while nobody was watching finished without us -
                //no closing word half an hour after the game started.
                _dropMissingOnce = true;

                _timer = new Timer(OnTick, null, TimeSpan.Zero, Tick);
            }
            else
            {
                _timer?.Dispose();
                _timer = null;

                foreach (var finished in _tracks.Values
                             .Where(a => a.FinishedUtc.HasValue)
                             .Select(a => a.ExeId)
                             .ToList())
                {
                    _tracks.Remove(finished);
                    _dismissed.Remove(finished);
                }

                Recompute();
            }
        }

        //Arrives from JS interop, and ApplyTimer touches _tracks, which the scan only
        //ever reads on the UI thread.
        private void OnActivityChanged() => DispatchWorkflow(() =>
        {
            ApplyTimer();
            return Task.CompletedTask;
        });

        public override void Dispose()
        {
            TopBannerArbiter.SetDeploymentVisible(false);

            //Static event, outlives the component: unsubscribe before dropping the timer.
            ShellActivity.Changed -= OnActivityChanged;

            _timer?.Dispose();
            _timer = null;

            base.Dispose();
        }

        #endregion

        #region EVENTS

        //Timer callbacks arrive on a pool thread. This must not be async void: an
        //exception from one would come back on the pool and take the whole client
        //down with it (see ShellComponentBase.DispatchWorkflow).
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

            foreach (var track in _tracks.Values.ToList())
            {
                if (activeIds.Contains(track.ExeId))
                    continue;

                if (_dropMissingOnce)
                {
                    _tracks.Remove(track.ExeId);
                    _dismissed.Remove(track.ExeId);
                    continue;
                }

                track.FinishedUtc ??= now;
            }

            _dropMissingOnce = false;

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

            //The slot is shared; an advertisement waits while this is in it.
            TopBannerArbiter.SetDeploymentVisible(_open);

            if (_done)
            {
                _title = ShellStringOverrides.Get(ShellStringOverrides.GEN_DONE);
                _subtitle = justDone.Count == 1
                    ? ShellStringOverrides.Get(ShellStringOverrides.DEPLOY_LAUNCHING, Shorten(justDone[0].Caption))
                    : ShellStringOverrides.GetPlural(ShellStringOverrides.DEPLOY_READY_COUNT, justDone.Count);
                _indeterminate = false;
                _percent = 100;
            }
            else if (live.Count > 0)
            {
                //"Deployment" only when there is a deployment profile behind it; anything
                //else preparing itself is just "Preparing".
                _title = ShellStringOverrides.Get(live.Any(a => a.HasDeploymentProfile)
                    ? ShellStringOverrides.DEPLOY_TITLE
                    : ShellStringOverrides.DEPLOY_TITLE_PREPARING);

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
                        ? ShellStringOverrides.Get(ShellStringOverrides.DEPLOY_COPYING)
                        : Shorten(live[0].Caption);
                }
                else
                {
                    //Anything longer does not fit the pill - the rest is behind the click.
                    _subtitle = ShellStringOverrides.GetPlural(ShellStringOverrides.DEPLOY_RUNNING_COUNT, live.Count);
                }
            }

            var signature = $"{_open}|{_done}|{_indeterminate}|{_percent}|{_title}|{_subtitle}";
            if (signature == _signature)
                return;

            _signature = signature;

            DispatchRender();
        }

        private static string Shorten(string value) =>
            string.IsNullOrEmpty(value) || value.Length <= 34 ? value : value[..33].TrimEnd() + "…";

        #endregion
    }
}
