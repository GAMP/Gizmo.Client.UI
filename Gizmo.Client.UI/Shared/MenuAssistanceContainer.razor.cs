using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuAssistanceContainer : CustomDOMComponentBase, IAsyncDisposable
    {
        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        AssistanceRequestViewService AssistanceRequestViewService { get; set; }

        [Inject]
        AssistanceRequestViewState ViewState { get; set; }

        [Inject]
        UserMenuViewState UserMenuViewState { get; set; }

        [Inject]
        UserMenuViewService UserMenuViewService { get; set; }

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        #endregion

        protected string MenuClass => UserMenuViewState.AssistanceRequestsIsVisible ? "giz-dropdown-menu open" : "giz-dropdown-menu";

        protected string NoteCount => $"{ViewState.Note?.Length ?? 0}/{ViewState.NoteLimit}";

        protected string ReasonClass(int id) => ViewState.SelectedAssistanceRequestType == id
            ? "giz-menu-assistance__reason giz-menu-assistance__reason--on"
            : "giz-menu-assistance__reason";

        protected string ReasonPressed(int id) => ViewState.SelectedAssistanceRequestType == id ? "true" : "false";

        private void ValueChangedHandler(int? value)
        {
            AssistanceRequestViewService.SetSelectedAssistanceRequestType(value);
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserMenuViewState);

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
            this.UnsubscribeChange(UserMenuViewState);
            this.UnsubscribeChange(ViewState);

            ClosePopupEventInterop?.Dispose();

            base.Dispose();
        }

        private ClosePopupEventInterop ClosePopupEventInterop { get; set; }

        private Task ClosePopupHandler(string args)
        {
            if (args == Id)
            {
                UserMenuViewService.CloseAssistanceRequests();
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
