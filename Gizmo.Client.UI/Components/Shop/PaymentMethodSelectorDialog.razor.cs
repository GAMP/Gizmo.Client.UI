using Gizmo.Client.UI.View.Services;
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
        }

        private bool _isOpen { get; set; }

        [Inject]
        UserCartService UserCartService { get; set; }

        [Inject]
        PaymentMethodsService PaymentMethodsService { get; set; }

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

            if (UserCartService.ViewState.IsComplete)
                return UserCartService.ResetAsync();

            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(UserCartService.ViewState);
            this.SubscribeChange(PaymentMethodsService.ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(UserCartService.ViewState);
            this.SubscribeChange(PaymentMethodsService.ViewState);
            base.Dispose();
        }
    }
}