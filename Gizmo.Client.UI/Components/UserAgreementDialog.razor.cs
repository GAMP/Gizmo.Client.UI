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

        #endregion

        #region METHODS

        private Task CloseDialog()
        {
            return CancelCallback.InvokeAsync();
        }

        #endregion
    }
}