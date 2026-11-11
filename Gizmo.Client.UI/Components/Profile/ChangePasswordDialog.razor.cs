using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangePasswordDialog : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        UserChangePasswordViewService UserChangePasswordService { get; set; }

        [Inject]
        UserChangePasswordViewState ViewState { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<EmptyComponentResult> ResultCallback { get; set; }

        protected string DialogClass => !ViewState.IsComplete
            ? "giz-tile-dialog"
            : ViewState.HasError ? "giz-tile-dialog giz-tile-dialog--danger" : "giz-tile-dialog giz-tile-dialog--success";

        protected string IconClass => !ViewState.IsComplete
            ? "ph-bold ph-key"
            : ViewState.HasError ? "ph-bold ph-warning-octagon" : "ph-bold ph-check-circle";

        protected string Title => GrafitLocalization.GetString(!ViewState.IsComplete
            ? GrafitResourceKeys.SHELL_PASSWORD_TITLE
            : ViewState.HasError ? GrafitResourceKeys.SHELL_PASSWORD_FAILED : GrafitResourceKeys.SHELL_PASSWORD_DONE);

        protected string CloseLabel => GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_CLOSE));

        protected bool IsSaveDisabled => ViewState.IsValid != true || ViewState.IsLoading;

        private async Task CloseDialog()
        {
            await ResultCallback.InvokeAsync();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
