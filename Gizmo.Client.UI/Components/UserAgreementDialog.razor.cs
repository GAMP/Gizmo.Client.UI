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

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        [Parameter]
        public EventCallback<UserAgreementResult> ResultCallback { get; set; }

        #endregion

        #region METHODS

        private Task CloseDialogAsync()
        {
            return CancelCallback.InvokeAsync();
        }

        private Task AcceptCurrentAgreementAsync()
        {
            return ResultCallback.InvokeAsync(new UserAgreementResult());
        }

        #endregion
    }
}