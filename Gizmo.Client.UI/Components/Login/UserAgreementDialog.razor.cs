using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class UserAgreementDialog : CustomDOMComponentBase
    {
        private bool _accepted;
        private bool _isRead;
        private ElementReference _text;
        private DotNetObjectReference<UserAgreementDialog> _self;
        private string _watched;
        private bool _watch;

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Agreement { get; set; }

        [Parameter]
        public bool IsRejectable { get; set; }

        [Parameter]
        public bool AllowContinueWithoutAccept { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<UserAgreementResult> ResultCallback { get; set; }

        protected string ClassName => AllowContinueWithoutAccept
            ? "giz-user-agreement-dialog giz-user-agreement-dialog--signup"
            : "giz-user-agreement-dialog";

        protected string Title => string.IsNullOrWhiteSpace(Name)
            ? LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_AGREEMENT_DIALOG_TITLE))
            : Name;

        protected bool CanDecline => IsRejectable || AllowContinueWithoutAccept;

        protected bool IsRead => _isRead;

        protected string DeclineText => GrafitLocalization.GetString(IsRejectable
            ? GrafitResourceKeys.SHELL_AGREEMENT_SKIP
            : GrafitResourceKeys.SHELL_AGREEMENT_DECLINE);

        [JSInvokable]
        public Task ReadToEnd()
        {
            _isRead = true;
            return InvokeAsync(StateHasChanged);
        }

        private async Task ScrollPageAsync()
        {
            try
            {
                await InvokeVoidAsync("scrollAgreementPage", _text);
            }
            catch (JSException)
            {
                await ReadToEnd();
            }
        }

        private Task CloseDialogAsync() => DismissCallback.InvokeAsync();

        private Task AcceptAsync()
        {
            _accepted = true;
            return ContinueAsync();
        }

        private Task DeclineAsync()
        {
            _accepted = false;
            return ContinueAsync();
        }

        private Task ContinueAsync() => ResultCallback.InvokeAsync(new UserAgreementResult() { Accepted = _accepted });

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _accepted = false;

            if (_watched != Agreement)
            {
                _watched = Agreement;
                _isRead = false;
                _watch = true;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!_watch)
                return;

            _watch = false;
            _self ??= CreateDotNetObjectReference(this);

            try
            {
                await InvokeVoidAsync("watchAgreementReading", _text, _self);
            }
            catch (JSException)
            {
                await ReadToEnd();
            }
        }

        public override void Dispose()
        {
            _self?.Dispose();
            base.Dispose();
        }
    }
}
