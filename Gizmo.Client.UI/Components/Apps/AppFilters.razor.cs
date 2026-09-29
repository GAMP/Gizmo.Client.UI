using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class AppFilters : CustomDOMComponentBase
    {
        #region PROPERTIES

        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        AppsPageViewService AppsPageService { get; set; }

        [Inject()]
        public AppsPageViewState ViewState { get; set; }

        #endregion

        private bool HasSearch => !string.IsNullOrEmpty(ViewState.SearchPattern);

        private bool HasRow => ViewState.AppCategories.Any() || ViewState.ExecutableModes.Any();

        private string AllText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_FILTERS_ALL);

        private string ResetText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_FILTERS_RESET);

        private string Heading => LocalizationService.GetString(ViewState.SelectedSortingOption switch
        {
            Gizmo.Client.ApplicationSortingOption.AddDate => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_APP_FILTERS_RECENTLY_ADDED),
            Gizmo.Client.ApplicationSortingOption.ReleaseDate => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_APP_FILTERS_NEW_RELEASES),
            _ => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_APP_FILTERS_ALL_APPS),
        });

        private string SortClass(Gizmo.Client.ApplicationSortingOption option) =>
            option == ViewState.SelectedSortingOption
                ? "giz-app-filters__seg giz-app-filters__seg--on"
                : "giz-app-filters__seg";

        private string CategoryClass(int? categoryId) =>
            categoryId == ViewState.SelectedCategoryId
                ? "giz-app-filters__chip giz-app-filters__chip--on"
                : "giz-app-filters__chip";

        private bool IsModeOn(ApplicationModes mode) => ViewState.SelectedExecutableModes.Contains(mode);

        private string ModeClass(ApplicationModes mode) =>
            IsModeOn(mode)
                ? "giz-app-filters__chip giz-app-filters__chip--mode giz-app-filters__chip--on"
                : "giz-app-filters__chip giz-app-filters__chip--mode";

        private string ModeIcon(ApplicationModes mode) => IsModeOn(mode) ? "ph-bold ph-check" : "ph-bold ph-plus";

        private string ModePressed(ApplicationModes mode) => IsModeOn(mode) ? "true" : "false";

        private Task ToggleMode(ApplicationModes mode)
        {
            var selected = ViewState.SelectedExecutableModes.ToList();

            if (!selected.Remove(mode))
                selected.Add(mode);

            return AppsPageService.SetSelectedSelectedExecutableModes(selected);
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
