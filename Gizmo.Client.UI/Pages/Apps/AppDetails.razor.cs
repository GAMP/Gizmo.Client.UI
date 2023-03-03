using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.ApplicationDetailsRoute)]
    public partial class AppDetails : CustomDOMComponentBase
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
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ApplicationDetailsPageService ApplicationDetailsPageService { get; set; }

        [Inject]
        ExecutableSelectorService ExecutableSelectorService { get; set; }

        [Inject]
        public ActiveApplicationsService ActiveApplicationsService { get; set; }

        [Inject()]
        IClientDialogService DialogService { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int ApplicationId { get; set; }

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
            if (ApplicationDetailsPageService.ViewState.Application.Executables.Count() == 1)
            {
                var executable = ApplicationDetailsPageService.ViewState.Application.Executables.First();

                switch (executable.State)
                {
                    case View.ExecutableState.None:
                        await ActiveApplicationsService.RunExecutableAsyc(executable.Id);

                        break;

                    default:
                        await ActiveApplicationsService.TerminateExecutableAsyc(executable.Id);

                        break;
                }
            }
            else if (ApplicationDetailsPageService.ViewState.Application.Executables.Count() > 1)
            {
                await ExecutableSelectorService.LoadApplicationAsync(ApplicationId);

                var s = await DialogService.ShowExecutableSelectorDialogAsync();
                if (s.Result == DialogAddResult.Success)
                {
                    try
                    {
                        var result = await s.WaitForDialogResultAsync();
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            if (ApplicationDetailsPageService.ViewState.Application.Executables.Count() == 1)
            {
                this.SubscribeChange(ApplicationDetailsPageService.ViewState.Application.Executables.First());
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (ApplicationDetailsPageService.ViewState.Application.Executables.Count() == 1)
            {
                this.UnsubscribeChange(ApplicationDetailsPageService.ViewState.Application.Executables.First());
            }

            base.Dispose();
        }
    }
}
