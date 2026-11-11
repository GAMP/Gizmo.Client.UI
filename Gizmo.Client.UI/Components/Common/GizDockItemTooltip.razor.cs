using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
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

        [Inject]
        AppViewStateLookupService AppViewStateLookupService { get; set; }

        [Parameter]
        public AppExeViewState Executable { get; set; }

        [Parameter]
        public bool IsOpen { get; set; }

        private AppViewState _app;

        protected int? CoverImageId => _app?.ImageId;

        protected int CoverKey => Executable?.ApplicationId ?? 0;

        protected string DetailsLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_DETAILS));

        protected override async Task OnParametersSetAsync()
        {
            if (Executable is not null && _app?.ApplicationId != Executable.ApplicationId)
                _app = await AppViewStateLookupService.GetStateAsync(Executable.ApplicationId);

            await base.OnParametersSetAsync();
        }

        private void OnClickDetailsButtonHandle()
        {
            if (Executable != null)
                NavigationService.NavigateTo(ClientRoutes.ApplicationDetailsRoute + $"?ApplicationId={Executable.ApplicationId.ToString()}");
        }
    }
}
