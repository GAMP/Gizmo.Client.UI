using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuNotificationsContainer : CustomDOMComponentBase, IAsyncDisposable
    {
        const int DEFAULT_DELAY =  500;
        
        public MenuNotificationsContainer()
        {
            _fadeInDeferredAction = new DeferredAction(FadeIn);
            _fadeOutDeferredAction = new DeferredAction(FadeOut);
            _delayTimeSpan = new TimeSpan(0, 0, 0, 0, _delay);
        }

        private bool _isOpen;
        private bool _isFadingIn;
        private bool _isFadingOut;

        private DeferredAction _fadeInDeferredAction;
        private DeferredAction _fadeOutDeferredAction;
        private int _delay = DEFAULT_DELAY;
        private TimeSpan _delayTimeSpan;

        #region PROPERTIES

        [Inject]
        NotificationsService NotificationsService { get; set; }

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value && !_isFadingOut)
                    return;

                if (value)
                {
                    if (_isFadingOut)
                    {
                        _isFadingOut = false;
                        _fadeOutDeferredAction.Cancel();
                    }

                    _isOpen = value;
                    _ = IsOpenChanged.InvokeAsync(_isOpen);
                    _ = InvokeVoidAsync("expandElement", Ref);

                    _isFadingIn = true;
                    _fadeInDeferredAction.Defer(_delayTimeSpan);
                }
                else
                {
                    if (_isFadingIn)
                    {
                        _isFadingIn = false;
                        _fadeInDeferredAction.Cancel();
                    }

                    if (!_isFadingOut)
                    {
                        _isFadingOut = true;
                        _fadeOutDeferredAction.Defer(_delayTimeSpan);
                        _ = InvokeVoidAsync("collapseElement", Ref);
                    }
                }
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        #endregion

        private Task FadeIn()
        {
            _isFadingIn = false;

            return Task.CompletedTask;
        }

        private Task FadeOut()
        {
            InvokeAsync(Close);

            return Task.CompletedTask;
        }

        private Task Close()
        {
            _isOpen = false;
            _isFadingOut = false;

            //StateHasChanged();

            return IsOpenChanged.InvokeAsync(_isOpen);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(NotificationsService.ViewState);
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await JsRuntime.InvokeVoidAsync("registerPopup", Ref);
                ClosePopupEventInterop = new ClosePopupEventInterop(JsRuntime);
                await ClosePopupEventInterop.SetupClosePopupEventCallback(args => ClosePopupHandler(args));
            }
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(NotificationsService.ViewState);

            ClosePopupEventInterop?.Dispose();

            base.Dispose();
        }

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private Task ClosePopupHandler(string args)
        {
            if (args == Id)
                IsOpen = false;

            return Task.CompletedTask;
        }

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            await InvokeVoidAsync("unregisterPopup", Ref).ConfigureAwait(false);

            Dispose();
        }

        #endregion
    }
}