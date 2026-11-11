using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuActiveApplicationCard : CustomDOMComponentBase
    {
        private AppExeExecutionViewState _execution;
        private IReadOnlyList<PersonalFileViewState> _files = new List<PersonalFileViewState>();
        private bool _filesOpen;

        [Inject]
        AppExecutionService AppExecutionService { get; set; }

        [Inject]
        AppExeExecutionViewStateLookupService AppExeExecutionViewStateLookupService { get; set; }

        [Inject]
        PersonalFileViewStateLookupService PersonalFileViewStateLookupService { get; set; }

        [CascadingParameter]
        GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter]
        public AppExeViewState Executable { get; set; }

        protected bool IsBusy => _execution?.IsActive == true;

        protected bool CanLaunchAgain => !IsBusy && Executable.Options.HasFlag(ExecutableOptionType.MultiRun);

        protected bool HasFiles => _files.Count > 0;

        protected IReadOnlyList<PersonalFileViewState> Files => _files;

        protected bool ShowFiles => _filesOpen && _files.Count > 1;

        protected string FilesClass => ShowFiles
            ? "giz-active-app-card__act giz-active-app-card__act--open"
            : "giz-active-app-card__act";

        protected string FilesExpanded => ShowFiles ? "true" : "false";

        protected string StatusText
        {
            get
            {
                if (_execution is null || !_execution.IsActive)
                    return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_APPS_RUNNING);

                if (_execution.IsIndeterminate)
                    return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_APPS_STARTING);

                return GrafitLocalization.GetString(GrafitResourceKeys.SHELL_APPS_LOADING,
                    decimal.Round(_execution.Progress).ToString("0", CultureInfo.CurrentCulture));
            }
        }

        protected string AutoLaunchClass => Executable.AutoLaunch
            ? "giz-active-app-card__act giz-active-app-card__act--on"
            : "giz-active-app-card__act";

        protected Task ToggleAutoLaunch() =>
            AppExecutionService.SetAutoLaunchAsync(Executable.ExecutableId, !Executable.AutoLaunch);

        protected Task Repair() =>
            AppExecutionService.AppExeExecuteAsync(Executable.ExecutableId, true, default);

        protected Task LaunchAgain() =>
            AppExecutionService.AppExeExecuteAsync(Executable.ExecutableId, false, default);

        protected Task Terminate() =>
            AppExecutionService.AppExeTerminateAsync(Executable.ExecutableId);

        protected Task Abort() =>
            AppExecutionService.AppExeAbortAsync(Executable.ExecutableId);

        protected Task OpenFiles()
        {
            if (_files.Count == 1)
                return OpenFile(_files[0]);

            _filesOpen = !_filesOpen;
            return Task.CompletedTask;
        }

        protected Task OpenFile(PersonalFileViewState file) =>
            AppExecutionService.PersonalFileExploreAsync(Executable.ExecutableId, file.PersonalFileId);

        #region OVERRIDES

        protected override async Task OnInitializedAsync()
        {
            if (Executable != null)
            {
                this.SubscribeChange(Executable);

                _execution = await AppExeExecutionViewStateLookupService.GetStateAsync(Executable.ExecutableId);

                if (_execution != null)
                    this.SubscribeChange(_execution);

                var files = new List<PersonalFileViewState>();

                foreach (var file in Executable.PersonalFiles)
                    files.Add(await PersonalFileViewStateLookupService.GetStateAsync(file.PersonalFileId));

                _files = files.Where(a => a != null).ToList();
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_execution != null)
                this.UnsubscribeChange(_execution);

            if (Executable != null)
                this.UnsubscribeChange(Executable);

            base.Dispose();
        }

        #endregion
    }
}
