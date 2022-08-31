using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route("/appdetails/{ApplicationId:int}")]
    public partial class AppDetails : ComponentBase
    {
        public AppDetails()
        {
        }

        #region PROPERTIES

        [Inject]
        ApplicationDetailsPageService ApplicationDetailsPageService { get; set; }

        [Inject]
        ExecutableSelectorService ExecutableSelectorService { get; set; }

        [Parameter]
        public int ApplicationId { get; set; }

        public bool ExecutableSelectorIsOpen { get; set; }

        #endregion

        #region METHODS

        #endregion

        private async Task OnClickMainButtonHandler()
        {
            if (ApplicationDetailsPageService.ViewState.Application.Executables.Count > 1)
            {
                await ExecutableSelectorService.LoadApplicationAsync(ApplicationId);
                ExecutableSelectorIsOpen = true;
            }
            else
            {
                //TODO: A LAUNCH THE EXE?
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await ApplicationDetailsPageService.LoadApplicationAsync(ApplicationId);

            await base.OnInitializedAsync();
        }
    }
}