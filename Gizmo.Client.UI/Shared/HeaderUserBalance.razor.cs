using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI
{
    public partial class HeaderUserBalance : ComponentBase , IDisposable
    {
        private bool _hideBalance;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserBalanceViewState UserBalanceViewState { get; set; }

        private void ToggleBalanceVisibility()
        {
            _hideBalance = !_hideBalance;
        }

        protected override Task OnInitializedAsync()
        {
            this.SubscribeChange(UserBalanceViewState);
            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            this.UnsubscribeChange(UserBalanceViewState);
        }
    }
}
