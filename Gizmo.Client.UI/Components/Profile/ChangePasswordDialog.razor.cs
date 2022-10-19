using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangePasswordDialog : CustomDOMComponentBase
    {
        public ChangePasswordDialog()
        {
        }

        private bool _isOpen { get; set; }

        [Inject]
        UserChangePasswordService UserChangePasswordService { get; set; }

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

        private Task CloseDialog()
        {
            IsOpen = false;

            if (UserChangePasswordService.ViewState.IsComplete)
                return UserChangePasswordService.ResetAsync();

            return Task.CompletedTask;

        }
    }
}