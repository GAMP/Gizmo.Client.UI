using System.Threading.Tasks;
using Gizmo.UI.View.States;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class DialogHost : CustomDOMComponentBase
    {
        private bool _previouslyTrapped = false;

        [Inject()]
        DialogHostViewState ViewState { get; init; }

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!_previouslyTrapped && ViewState.HasDialog)
            {
                await InvokeVoidAsync("focusTrap", Ref);
                _previouslyTrapped = true;
            }
            else if(_previouslyTrapped && !ViewState.HasDialog)
            {
                await InvokeVoidAsync("focusUntrap", Ref);
                _previouslyTrapped = false;
            }
        }
    }
}
