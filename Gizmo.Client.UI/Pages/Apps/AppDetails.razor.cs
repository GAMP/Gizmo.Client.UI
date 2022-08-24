using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;

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

        private void OnClickMainButtonHandler()
        {
            if (ApplicationDetailsPageService.ViewState.Application.Executables.Count > 1)
            {
                ExecutableSelectorService.SetApplication(ApplicationId);
                ExecutableSelectorIsOpen = true;
            }
            else
            {
                //TODO: A LAUNCH THE EXE?
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ApplicationDetailsPageService.SetApplication(ApplicationId);
        }

    }
}