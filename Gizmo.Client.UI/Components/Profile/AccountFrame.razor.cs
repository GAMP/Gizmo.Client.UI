using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Components
{
    public partial class AccountFrame : CustomDOMComponentBase
    {
        [Inject]
        UserProfileViewState Profile { get; set; }

        [Inject]
        UserViewState UserViewState { get; set; }

        [Inject]
        UserBalanceViewState Balance { get; set; }

        [Inject]
        CreditOptionsViewState Credit { get; set; }

        [Inject]
        IOptionsMonitor<ClientInterfaceOptions> InterfaceOptions { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            this.SubscribeChange(Profile);
            this.SubscribeChange(UserViewState);
            this.SubscribeChange(Balance);
            this.SubscribeChange(Credit);
            Loyalty.Changed += OnLoyaltyChanged;

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Profile);
            this.UnsubscribeChange(UserViewState);
            this.UnsubscribeChange(Balance);
            this.UnsubscribeChange(Credit);
            Loyalty.Changed -= OnLoyaltyChanged;

            base.Dispose();
        }

        //The Progress tab appears or goes with the data; raised on the client's threads.
        private void OnLoyaltyChanged() => DispatchStateHasChanged();
    }
}
