using Gizmo.Client.UI.View.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class TopUpDialog : CustomDOMComponentBase
    {
        public TopUpDialog()
        {
        }

        private bool _isOpen { get; set; }

        [Inject]
        TopUpService TopUpService { get; set; }

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

        private void CloseDialog()
        {
            IsOpen = false;
        }
    }
}
