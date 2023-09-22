using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuUserLinks : CustomDOMComponentBase
    {
        protected bool _shouldRender;

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserViewService UserService { get; set; }

        [Inject]
        UserViewState ViewState { get; set; }

        [Inject]
        UserLockViewService UserLockService { get; set; }

        [Inject()]
        UserOnlineDepositViewService UserOnlineDepositViewStateService { get; set; }

        [Inject]
        UserMenuViewState UserMenuViewState { get; set; }

        [Inject]
        UserMenuViewService UserMenuViewService { get; set; }

        #endregion

        private Task OnClickUserLockButtonHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();

            return UserLockService.LockAsync();
        }

        private Task OnClickUserLogoutButtonHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();

            return UserService.LogoutWithConfirmationAsync();
        }

        private void OnClickLinkHandler()
        {
            _shouldRender = true;

            UserMenuViewService.CloseUserLinks();
        }

        #region OVERRIDES

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                _shouldRender = false;
                //await InvokeVoidAsync("writeLine", $"ReRender {this.ToString()}");
            }
            else
            {
                await JsRuntime.InvokeVoidAsync("registerPopup", Ref);
                ClosePopupEventInterop = new ClosePopupEventInterop(JsRuntime);
                await ClosePopupEventInterop.SetupClosePopupEventCallback(args => ClosePopupHandler(args));
            }
        }

        #endregion

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private Task ClosePopupHandler(string args)
        {
            if (args == Id)
            {
                _shouldRender = true;

                UserMenuViewService.CloseUserLinks();
            }

            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            ViewState.OnChange += ViewState_OnChange;
            UserMenuViewState.OnChange += ViewState_OnChange;

            base.OnInitialized();
        }

        private async void ViewState_OnChange(object sender, System.EventArgs e)
        {
            _shouldRender = true;
            await InvokeAsync(StateHasChanged);
        }

        public override void Dispose()
        {
            UserMenuViewState.OnChange -= ViewState_OnChange;
            ViewState.OnChange -= ViewState_OnChange;

            ClosePopupEventInterop?.Dispose();

            base.Dispose();
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
