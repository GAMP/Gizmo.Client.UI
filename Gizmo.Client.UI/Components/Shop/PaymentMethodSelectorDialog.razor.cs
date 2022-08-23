using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class PaymentMethodSelectorDialog : CustomDOMComponentBase
    {
        public PaymentMethodSelectorDialog()
        {
            PaymentMethods = new List<PaymentMethodViewState>();
            //TODO: A 
            //PaymentMethods.Add(new PaymentMethodViewState() { Id = 1, Name = "Cash", Icon = Icons.Coins_Client });
            //PaymentMethods.Add(new PaymentMethodViewState() { Id = 2, Name = "Points", Icon = Icons.Trophy_Client });
        }

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
        public EventCallback<int> OnSelectPaymentMethod { get; set; }

        public List<PaymentMethodViewState> PaymentMethods { get; set; }

        private void CloseDialog()
        {
            IsOpen = false;
        }

        private async Task SelectPaymentMethod(int id)
        {
            await OnSelectPaymentMethod.InvokeAsync(id);
            IsOpen = false;
        }

    }
}