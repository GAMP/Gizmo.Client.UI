using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.Services;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuUserLinks : CustomDOMComponentBase
    {
        public MenuUserLinks()
        {
        }

        private bool _isOpen;

        protected bool _shouldRender;

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserViewStateService UserService { get; set; }

        [Inject]
        UserLockViewStateService UserLockService { get; set; }

        [Inject()]
        TopUpViewStateService TopUpService { get; set; }

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;
                _ = IsOpenChanged.InvokeAsync(_isOpen);
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        #endregion

        //private async Task OnClickTopUpButtonHandler()
        //{
        //    _shouldRender = true;

        //    IsOpen = false;

        //    await TopUpService.ShowDialogAsync();
        //}

        private Task OnClickUserLockButtonHandler()
        {
            _shouldRender = true;

            IsOpen = false;

            return UserLockService.LockAsync();
        }

        private Task OnClickUserLogoutButtonHandler()
        {
            _shouldRender = true;

            IsOpen = false;

            return UserService.LogοutAsync();
        }

        private void OnClickLinkHandler()
        {
            _shouldRender = true;

            IsOpen = false;
        }

        #region OVERRIDES

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<bool>(nameof(IsOpen), out var newIsOpen))
            {
                var isOpenChanged = IsOpen != newIsOpen;
                if (isOpenChanged)
                {
                    _shouldRender = true;
                }
            }

            await base.SetParametersAsync(parameters);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                _shouldRender = false;
                await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                await JsRuntime.InvokeVoidAsync("registerPopup", Ref);
                ClosePopupEventInterop = new ClosePopupEventInterop(JsRuntime);
                await ClosePopupEventInterop.SetupClosePopupEventCallback(args => ClosePopupHandler(args));
            }
        }

        public override void Dispose()
        {
            ClosePopupEventInterop?.Dispose();

            base.Dispose();
        }

        #endregion

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private Task ClosePopupHandler(string args)
        {
            if (args == Id)
            {
                _shouldRender = true;

                IsOpen = false;
            }

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
