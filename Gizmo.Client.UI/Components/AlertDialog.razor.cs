using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class AlertDialog : CustomDOMComponentBase
    {
        private bool _isOpen { get; set; }

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
        public AlertTypes AlertType { get; set; } = AlertTypes.Warning;

        [Parameter]
        public string Message { get; set; }

        private void CloseDialog()
        {
            IsOpen = false;
        }

        public enum AlertTypes
        {
            Info,
            Warning,
            Error
        }

    }
}