using System;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client.UI.Services;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class AvatarNudgeBanner : ShellComponentBase
    {
        private static readonly TimeSpan AutoHideDelay = TimeSpan.FromSeconds(12);

        [Inject]
        IClientDialogService DialogService { get; set; }

        //An advertisement gives way to business: while a deployment runs or the level
        //hint is out, the slot belongs to them.
        protected bool IsVisible => AvatarService.Current?.ShowNudge == true && !TopBannerArbiter.SlotTaken;

        private CancellationTokenSource _autoHideCts;

        protected override void OnInitialized()
        {
            if (AvatarService.Current is not null)
                AvatarService.Current.Changed += OnAvatarChanged;

            TopBannerArbiter.Changed += OnSlotChanged;

            base.OnInitialized();
        }

        //A static event outlives the component: unsubscribing is not optional (Dispose).
        private void OnSlotChanged() => DispatchRender();

        //Raised from the picture service's HTTP continuation, never on the renderer's
        //thread - and never as async void; see ShellDispatch for the crash that rule
        //comes from.
        private void OnAvatarChanged()
        {
            if (IsVisible)
                StartAutoHide();
            else
                _autoHideCts?.Cancel();

            DispatchRender();
        }

        private Task OnClick()
        {
            if (!IsVisible)
                return Task.CompletedTask;

            _autoHideCts?.Cancel();
            AvatarService.Current?.DismissNudge();

            return DialogService.ShowChangePictureDialogAsync();
        }

        private void StartAutoHide()
        {
            _autoHideCts?.Cancel();
            _autoHideCts = new CancellationTokenSource();

            _ = AutoHideAsync(_autoHideCts.Token);
        }

        private async Task AutoHideAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(AutoHideDelay, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                    AvatarService.Current?.DismissNudge();
            }
            catch (OperationCanceledException)
            {
                //Dismissed or replaced before the delay was out.
            }
            catch (ObjectDisposedException)
            {
                //The token source went away with the component.
            }
        }

        public override void Dispose()
        {
            _autoHideCts?.Cancel();

            if (AvatarService.Current is not null)
                AvatarService.Current.Changed -= OnAvatarChanged;

            TopBannerArbiter.Changed -= OnSlotChanged;

            base.Dispose();
        }
    }
}
