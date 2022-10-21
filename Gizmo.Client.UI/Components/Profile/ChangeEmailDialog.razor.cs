using Gizmo.Client.UI.View.Services;
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

        private bool _isOpen { get; set; }

        [Inject]
        UserChangeEmailService UserChangeEmailService { get; set; }

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

            if (UserChangeEmailService.ViewState.IsComplete)
                return UserChangeEmailService.ResetAsync();

            return Task.CompletedTask;
        }
    }
}