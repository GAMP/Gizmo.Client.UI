using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class UserAgreementDialog : CustomDOMComponentBase
    {
        public UserAgreementDialog()
        {
        }

        #region PROPERTIES

        [Inject]
        UserAgreementsService UserAgreementsService { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        [Parameter]
        public EventCallback<UserAgreementResult> ResultCallback { get; set; }

        #endregion

        #region METHODS

        private async Task CloseDialogAsync()
        {
            await CancelCallback.InvokeAsync();
        }

        private async Task AcceptCurrentAgreementAsync()
        {
            await ResultCallback.InvokeAsync(new UserAgreementResult());
        }

        #endregion
    }
}