using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class UserAgreementDialog : CustomDOMComponentBase
    {
        private bool _accepted;

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

        protected string DeclineText => GrafitLocalization.GetString(IsRejectable
            ? GrafitResourceKeys.SHELL_AGREEMENT_SKIP
            : GrafitResourceKeys.SHELL_AGREEMENT_DECLINE);


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
        }
    }
}
