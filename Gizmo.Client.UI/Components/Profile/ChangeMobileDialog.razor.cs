using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ChangeMobileDialog : CustomDOMComponentBase
    {
        public ChangeMobileDialog()
        {
        }

        private bool _isOpen { get; set; }

        [Inject]
        UserChangeMobileService UserChangeMobileService { get; set; }

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

            if (UserChangeMobileService.ViewState.IsComplete)
                return UserChangeMobileService.ResetAsync();

            return Task.CompletedTask;
        }
    }
}