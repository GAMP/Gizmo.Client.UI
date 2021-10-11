using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Components
{
    public partial class SelectPaymentMethodDialog : CustomDOMComponentBase
    {
        public SelectPaymentMethodDialog()
        {
            PaymentMethods = new List<PaymentMethodViewModel>();
            PaymentMethods.Add(new PaymentMethodViewModel() { Id = 1, Name = "Cash", Icon = Icons.Coins_Client });
            PaymentMethods.Add(new PaymentMethodViewModel() { Id = 2, Name = "Points", Icon = Icons.Trophy_Client });
        }

        private bool _isOpen { get; set; }

        private int _selectedPaymentMethod = 2;

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

        public List<PaymentMethodViewModel> PaymentMethods { get; set; }

        private void CloseDialog()
        {
            IsOpen = false;
        }

    }
}
