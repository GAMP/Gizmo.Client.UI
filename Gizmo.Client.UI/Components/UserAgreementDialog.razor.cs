using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class UserAgreementDialog : CustomDOMComponentBase
    {
        public UserAgreementDialog()
        {
        }

        #region FIELDS

        private bool _isOpen { get; set; }

        #endregion

        #region PROPERTIES

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;

                _ = IsOpenChanged.InvokeAsync(_isOpen);
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public UserAgreementViewModel UserAgreement { get; set; }

        #endregion

        #region METHODS

        private void CloseDialog()
        {
            IsOpen = false;
        }

        #endregion
    }
}