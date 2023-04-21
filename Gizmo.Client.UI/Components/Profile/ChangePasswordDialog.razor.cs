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
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserChangePasswordViewStateService UserChangePasswordService { get; set; }

        [Inject]
        UserChangePasswordViewState ViewState { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private async Task CloseDialog()
        {
            await CancelCallback.InvokeAsync();
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
