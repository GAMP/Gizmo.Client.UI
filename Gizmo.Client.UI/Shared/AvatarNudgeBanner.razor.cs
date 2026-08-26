using Gizmo.Client.UI.Services;
using Gizmo.Web.Components;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class AvatarNudgeBanner : CustomDOMComponentBase
    {
        private static readonly TimeSpan AutoHideDelay = TimeSpan.FromSeconds(12);

        //Реклама уступает делу: пока идёт развёртывание, слот занят статусом
        //копирования файлов, а это плашка про «поставь фото» — подождёт.
        protected bool IsVisible => AvatarService.Current?.ShowNudge == true
                                    && !TopBannerArbiter.DeploymentVisible;

        private CancellationTokenSource _autoHideCts;

        protected override void OnInitialized()
        {
            if (AvatarService.Current != null)
                AvatarService.Current.Changed += OnAvatarChanged;

            TopBannerArbiter.Changed += OnSlotChanged;

            base.OnInitialized();
        }

        //Статическое событие переживает компонент, отписка обязательна - см. Dispose.
        private void OnSlotChanged() => DispatchStateHasChanged();

        //Raised from the avatar service's HTTP continuation, i.e. never on the UI thread.
        //Must not be async void: an InvokeAsync that faults while the WebView is being torn
        //down would be rethrown on the thread pool and kill the whole client (see
        //CustomComponentBase.DispatchWorkflow for the logged crash this comes from).
        private void OnAvatarChanged()
        {
            if (IsVisible)
            {
                StartAutoHideTimer();
            }
            else
            {
                _autoHideCts?.Cancel();
            }

            DispatchStateHasChanged();
        }

        private void StartAutoHideTimer()
        {
            _autoHideCts?.Cancel();
            _autoHideCts = new CancellationTokenSource();

            _ = AutoHideAsync(_autoHideCts.Token);
        }

        private async Task AutoHideAsync(CancellationToken cToken)
        {
            try
            {
                await Task.Delay(AutoHideDelay, cToken);

                if (!cToken.IsCancellationRequested)
                    AvatarService.Current?.DismissNudge();
            }
            catch (OperationCanceledException)
            {
                //banner was dismissed or replaced before the delay elapsed
            }
            catch (ObjectDisposedException)
            {
                //token source went away with the component
            }
        }

        public override void Dispose()
        {
            _autoHideCts?.Cancel();

            if (AvatarService.Current != null)
                AvatarService.Current.Changed -= OnAvatarChanged;

            TopBannerArbiter.Changed -= OnSlotChanged;

            base.Dispose();
        }
    }
}
