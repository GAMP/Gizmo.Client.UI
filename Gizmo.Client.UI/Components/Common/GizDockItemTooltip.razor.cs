using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class GizDockItemTooltip : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Parameter]
        public AppExeViewState Executable { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        private void OnClickDetailsButtonHandle()
        {
            if (Executable != null)
                NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute + $"?ApplicationId={Executable.ApplicationId.ToString()}");
        }
    }
}
