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
    public partial class DeploymentBanner : ShellComponentBase
    {
        private static readonly TimeSpan ShowAfter = TimeSpan.FromSeconds(3);

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

        private bool _dropMissingOnce;

        private bool _open;
        private bool _done;
        private bool _indeterminate;
        private int _percent;
        private string _title = string.Empty;
        private string _subtitle = string.Empty;
        private string _signature = string.Empty;

        private readonly HashSet<int> _dismissed = new();

        #endregion

        #region PROPERTIES

        protected bool IsOpen => _open;
        protected bool IsDone => _done;
        protected bool IsIndeterminate => _indeterminate;
        protected int PercentValue => _percent;
        protected string ProgressWidth => $"{(_done ? 100 : _percent)}%";
        protected string PercentLabel => _indeterminate ? "…" : $"{_percent}%";

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

        private void ApplyTimer()
        {
            var wanted = ShellActivity.IsActive;

            if (wanted == (_timer is not null))
                return;

            if (wanted)
            {
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

        private void OnActivityChanged() => DispatchWorkflow(() =>
        {
            ApplyTimer();
            return Task.CompletedTask;
        });

        public override void Dispose()
        {
            ShellActivity.Changed -= OnActivityChanged;

            _timer?.Dispose();
            _timer = null;

            base.Dispose();
        }

        #endregion

        #region EVENTS

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

            var live = _tracks.Values
                .Where(a => a.FinishedUtc == null
                            && now - a.StartedUtc >= ShowAfter
                            && !_dismissed.Contains(a.ExeId))
                .ToList();

            foreach (var track in live)
                track.WasShown = true;

            var justDone = _tracks.Values
                .Where(a => a.FinishedUtc.HasValue
                            && a.WasShown
                            && now - a.FinishedUtc.Value <= DoneLinger)
                .ToList();

            _open = live.Count > 0 || justDone.Count > 0;
            _done = live.Count == 0 && justDone.Count > 0;

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
                _title = ShellStringOverrides.Get(live.Any(a => a.HasDeploymentProfile)
                    ? ShellStringOverrides.DEPLOY_TITLE
                    : ShellStringOverrides.DEPLOY_TITLE_PREPARING);

                var measurable = live.Where(a => !a.Indeterminate).ToList();

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
