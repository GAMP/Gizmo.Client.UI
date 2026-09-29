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
        private const int CATEGORY_GRID_FROM = 9;

        private enum FilterMenu
        {
            None,
            Sort,
            Category,
            Mode,
        }

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

        private FilterMenu _open;

        private bool HasSearch => !string.IsNullOrEmpty(ViewState.SearchPattern);

        private string AllText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_FILTERS_ALL);

        private string ResetText => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_FILTERS_RESET);

        private string Heading => LocalizationService.GetString(ViewState.SelectedSortingOption switch
        {
            Gizmo.Client.ApplicationSortingOption.AddDate => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_APP_FILTERS_RECENTLY_ADDED),
            Gizmo.Client.ApplicationSortingOption.ReleaseDate => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_APP_FILTERS_NEW_RELEASES),
            _ => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_APP_FILTERS_ALL_APPS),
        });

        private string SortText => ViewState.SortingOptions
            .FirstOrDefault(option => option.Value == ViewState.SelectedSortingOption)?.DisplayName ?? string.Empty;

        private string CategoryText => ViewState.AppCategories
            .FirstOrDefault(category => category.AppCategoryId == ViewState.SelectedCategoryId)?.Name ?? AllText;

        private string ModeText
        {
            get
            {
                var selected = ViewState.ExecutableModes
                    .Where(mode => IsModeOn(mode.Value))
                    .Select(mode => mode.DisplayName)
                    .ToList();

                return selected.Count switch
                {
                    0 => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_FILTERS_ANY),
                    1 => selected[0],
                    _ => GrafitLocalization.GetString(GrafitResourceKeys.SHELL_FILTERS_SELECTED, selected.Count),
                };
            }
        }

        private string CategoryMenuClass => ViewState.AppCategories.Count() + 1 >= CATEGORY_GRID_FROM
            ? "giz-app-filter__menu giz-app-filter__menu--grid"
            : "giz-app-filter__menu";

        private bool IsOpen(FilterMenu menu) => _open == menu;

        private bool IsSet(FilterMenu menu) => menu switch
        {
            FilterMenu.Sort => ViewState.SelectedSortingOption != ViewState.DefaultSortingOption,
            FilterMenu.Category => ViewState.SelectedCategoryId.HasValue,
            FilterMenu.Mode => ViewState.SelectedExecutableModes.Any(),
            _ => false,
        };

        private string MenuClass(FilterMenu menu)
        {
            var classes = "giz-app-filter";

            if (IsSet(menu))
                classes += " giz-app-filter--set";

            if (IsOpen(menu))
                classes += " giz-app-filter--open";

            return classes;
        }

        private static string ItemClass(bool selected) => selected
            ? "giz-app-filter__item giz-app-filter__item--on"
            : "giz-app-filter__item";

        private string SortItemClass(Gizmo.Client.ApplicationSortingOption option) => ItemClass(option == ViewState.SelectedSortingOption);

        private string CategoryItemClass(int? categoryId) => ItemClass(categoryId == ViewState.SelectedCategoryId);

        private bool IsModeOn(ApplicationModes mode) => ViewState.SelectedExecutableModes.Contains(mode);

        private string ModeItemClass(ApplicationModes mode) => IsModeOn(mode)
            ? "giz-app-filter__item giz-app-filter__item--check giz-app-filter__item--on"
            : "giz-app-filter__item giz-app-filter__item--check";

        private string ModePressed(ApplicationModes mode) => IsModeOn(mode) ? "true" : "false";

        private void Toggle(FilterMenu menu) => _open = _open == menu ? FilterMenu.None : menu;

        private void Close() => _open = FilterMenu.None;

        private Task SelectSortAsync(Gizmo.Client.ApplicationSortingOption option)
        {
            _open = FilterMenu.None;
            return AppsPageService.SetSelectedSortingOption(option);
        }

        private Task SelectCategoryAsync(int? categoryId)
        {
            _open = FilterMenu.None;
            return AppsPageService.SetSelectedApplicationCategory(categoryId);
        }

        private Task ToggleMode(ApplicationModes mode)
        {
            var selected = ViewState.SelectedExecutableModes.ToList();

            if (!selected.Remove(mode))
                selected.Add(mode);

            return AppsPageService.SetSelectedSelectedExecutableModes(selected);
        }

        private Task ClearAllAsync()
        {
            _open = FilterMenu.None;
            return AppsPageService.ClearAllFilters();
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
