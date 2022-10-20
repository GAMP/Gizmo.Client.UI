using Gizmo.Client.UI.View.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route("/appdetails/{ApplicationId:int}")]
    public partial class AppDetails : ComponentBase
    {
        public AppDetails()
        {
            ApplicationMedia = Enumerable.Range(0, 5).Select(i => "app_media.png").ToList();
        }

        #region FIELDS
        private int _selectedTabIndex;
        #endregion

        #region PROPERTIES

        [Inject]
        ApplicationDetailsPageService ApplicationDetailsPageService { get; set; }

        [Inject]
        ExecutableSelectorService ExecutableSelectorService { get; set; }

        [Parameter]
        public int ApplicationId { get; set; }

        public bool ExecutableSelectorIsOpen { get; set; }

        public List<string> ApplicationMedia { get; set; }

        #endregion

        #region METHODS

        private Task SelectTab(int tabIndex)
        {
            _selectedTabIndex = tabIndex;

            StateHasChanged();

            return Task.CompletedTask;
        }

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