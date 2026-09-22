using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Shared
{
    /// <summary>
    /// Decides when the shell may stop rendering and running its timers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Losing focus is what puts the shell to sleep. It has to be: an application started
    /// outside Gizmo is something the shell never hears about, so any cleverer condition
    /// would miss the commonest case.
    /// </para>
    /// <para>
    /// One veto on top of that. A deployment is the only place in the shell where numbers
    /// move on their own and where a customer may reasonably be watching from another
    /// window, so while anything is being prepared nothing sleeps. The scan therefore runs
    /// only between losing focus and being cleared to sleep, and stops once asleep -
    /// preparation only starts from a launch, and a launch needs the shell in front.
    /// </para>
    /// <para>
    /// Uses a <see cref="DotNetObjectReference"/> rather than a static
    /// <c>[JSInvokable]</c>: the skin is loaded with <c>Assembly.LoadFrom</c> from the
    /// server's skins folder, and the object reference route is the one every other JS
    /// callback in this shell already relies on.
    /// </para>
    /// </remarks>
    public partial class ShellActivityWatcher : ShellComponentBase
    {
        /// <summary>Same cadence the file syncer publishes progress at.</summary>
        private static readonly TimeSpan ScanInterval = TimeSpan.FromSeconds(1);

        [Inject] AppExeExecutionViewStateLookupService ExecutionStates { get; set; }

        private DotNetObjectReference<ShellActivityWatcher> _selfRef;
        private Timer _scan;
        private bool _scanning;

        private bool _unfocused;
        private bool _idleAllowed;

        /// <summary>
        /// Called from the browser whenever focus or page visibility settles.
        /// </summary>
        [JSInvokable]
        public void OnShellFocusChanged(bool isFocused)
        {
            // Arrives from JS interop; everything below touches component state.
            DispatchWorkflow(() => ApplyFocusAsync(isFocused));
        }

        private async Task ApplyFocusAsync(bool isFocused)
        {
            _unfocused = !isFocused;

            if (isFocused)
            {
                // Sleeping is out of the question while the shell is in front, and the
                // previous answer is stale by the time it matters again.
                StopScan();
                await SetIdleAllowedAsync(false);
            }
            else
            {
                StartScan();
            }

            Publish();
        }

        // Due immediately: the answer is needed when focus leaves, not a second later.
        private void StartScan() => _scan ??= new Timer(OnScanTick, null, TimeSpan.Zero, ScanInterval);

        private void StopScan()
        {
            _scan?.Dispose();
            _scan = null;
        }

        // Not async void: a fault would come back on the pool and take the client down.
        private void OnScanTick(object _)
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

        private async Task ScanAsync()
        {
            IEnumerable<AppExeExecutionViewState> states;

            try
            {
                states = await ExecutionStates.GetStatesAsync();
            }
            catch
            {
                // Before login the lookup refuses outright. Not knowing means not sleeping.
                await SetIdleAllowedAsync(false);
                return;
            }

            var preparing = false;

            foreach (var state in states)
            {
                if (state.IsActive)
                {
                    preparing = true;
                    break;
                }
            }

            await SetIdleAllowedAsync(!preparing);

            if (_idleAllowed)
                StopScan();
        }

        private async Task SetIdleAllowedAsync(bool allowed)
        {
            if (allowed == _idleAllowed)
                return;

            _idleAllowed = allowed;

            // Own half first, so a teardown fault in interop cannot leave .NET stale.
            Publish();

            await InvokeVoidAsync("setShellIdleAllowed", allowed);
        }

        private void Publish() => ShellActivity.SetActive(!(_unfocused && _idleAllowed));

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _selfRef = CreateDotNetObjectReference(this);

                // Attaching answers with the current focus state, so nothing is guessed.
                await InvokeVoidAsync("attachShellActivity", _selfRef, nameof(OnShellFocusChanged));
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public override void Dispose()
        {
            StopScan();

            try
            {
                _ = InvokeVoidAsync("detachShellActivity");
            }
            catch
            {
                // JS runtime may already be gone.
            }

            // A shell with no watcher must not be left asleep.
            ShellActivity.SetActive(true);

            _selfRef?.Dispose();
            _selfRef = null;

            base.Dispose();
        }
    }
}
