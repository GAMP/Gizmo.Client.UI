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
    public partial class ShellActivityWatcher : ShellComponentBase
    {
        private static readonly TimeSpan ScanInterval = TimeSpan.FromSeconds(1);

        [Inject] AppExeExecutionViewStateLookupService ExecutionStates { get; set; }

        private DotNetObjectReference<ShellActivityWatcher> _selfRef;
        private Timer _scan;
        private bool _scanning;

        private bool _unfocused;
        private bool _idleAllowed;

        [JSInvokable]
        public void OnShellFocusChanged(bool isFocused)
        {
            DispatchWorkflow(() => ApplyFocusAsync(isFocused));
        }

        private async Task ApplyFocusAsync(bool isFocused)
        {
            _unfocused = !isFocused;

            if (isFocused)
            {
                StopScan();
                await SetIdleAllowedAsync(false);
            }
            else
            {
                StartScan();
            }

            Publish();
        }

        private void StartScan() => _scan ??= new Timer(OnScanTick, null, TimeSpan.Zero, ScanInterval);

        private void StopScan()
        {
            _scan?.Dispose();
            _scan = null;
        }

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

            Publish();

            await InvokeVoidAsync("setShellIdleAllowed", allowed);
        }

        private void Publish() => ShellActivity.SetActive(!(_unfocused && _idleAllowed));

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _selfRef = CreateDotNetObjectReference(this);

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
            }

            ShellActivity.SetActive(true);

            _selfRef?.Dispose();
            _selfRef = null;

            base.Dispose();
        }
    }
}
