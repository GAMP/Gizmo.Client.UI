using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableCard : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        public ActiveApplicationsService ActiveApplicationsService { get; set; }

        [Parameter]
        public AppExeViewState Executable { get; set; }

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(Executable);
            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Executable);
            base.Dispose();
        }

        #endregion
    }
}
