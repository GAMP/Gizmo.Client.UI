using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;
using System;

namespace Gizmo.Client.UI.Shared
{
    public partial class GizLanguageMenu : ComponentBase ,IDisposable
    {
        [Inject()]
        public ClientLanguageViewState ViewState
        {
            get;init;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.SubscribeChange(ViewState);
        }

        public void Dispose()
        {
            this.UnsubscribeChange(ViewState);
        }
    }
}
