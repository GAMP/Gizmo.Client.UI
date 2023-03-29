using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading;
using System;
using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;

namespace Gizmo.Client.UI.Components
{
    public partial class TopUpDialog : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        TopUpViewStateService TopUpService { get; set; }

        [Inject]
        TopUpViewState ViewState { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        private Task CloseDialog()
        {
            return CancelCallback.InvokeAsync();
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
