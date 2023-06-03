using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangeEmailDialog : CustomDOMComponentBase
    {
        public ChangeEmailDialog()
        {
        }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserChangeEmailViewService UserChangeEmailService { get; set; }

        [Inject]
        UserChangeEmailViewState ViewState { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        private async Task CloseDialog()
        {
            await DismissCallback.InvokeAsync();

            if (ViewState.IsComplete)
                await UserChangeEmailService.ResetAsync();
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
