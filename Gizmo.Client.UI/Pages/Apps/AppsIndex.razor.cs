using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_APPS)]
    [ModuleDisplayOrder(1)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_APPS_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_APPS_DESCRIPTION")]
    [DefaultRoute(ClientRoutes.ApplicationsRoute), Route(ClientRoutes.ApplicationsRoute)]
    public partial class AppsIndex : CustomDOMComponentBase
    {
        public AppsIndex()
        {
        }

        #region FIELDS
        private int _selectedSortOptionId = 1;
        private int? _selectedApplicationGroupId;
        private List<int> _selectedApplicationFilters = new List<int>() { 1, 3 };
        #endregion

        #region PROPERTIES

        [Inject]
        AppCategoryViewStateLookupService AppCategoryViewStateLookupService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ApplicationsPageService ApplicationsPageService { get; set; }

        [Inject]
        SearchService SearchService { get; set; }

        [Inject()]
        IClientDialogService DialogService { get; set; }

        #region PARAMETERS

        /// <summary>
        /// Application id url parameter.
        /// </summary>
        [Parameter()]
        public int? AppId
        {
            get;
            init;
        }

        #endregion

        public List<ApplicationSortOptionViewModel> ApplicationSortOptions { get; set; }

        public List<ApplicationFilterViewModel> ApplicationFilters { get; set; }

        public List<int> SelectedApplicationFilters
        {
            get
            {
                return _selectedApplicationFilters;
            }
            set
            {
                _selectedApplicationFilters = value;
            }
        }

        #endregion

        #region EVENTS

        public void SelectedApplicationFiltersChanged(List<int> selectedApplicationFilters)
        {
            SelectedApplicationFilters = selectedApplicationFilters;
        }

        public void OnClearFiltersHandler(string value)
        {
            _selectedApplicationGroupId = null;
            SelectedApplicationFilters.Clear();
        }

        #endregion

        #region METHODS

        public IEnumerable<AppViewState> GetFilteredApplications()
        {
            var result = ApplicationsPageService.ViewState.Applications.AsQueryable();

            if (SearchService.ViewState.ShowAll)
            {
                var ids = SearchService.ViewState.AppliedApplicationResults.Select(a => a.Id).ToList();

                result = result.Where(a => ids.Contains(a.ApplicationId));
            }

            if (_selectedApplicationGroupId.HasValue)
            {
                result = result.Where(a => a.ApplicationCategoryId == _selectedApplicationGroupId);
            }

            return result.ToList();
        }

        public int GetNumberOfFilters()
        {
            int result = 0;

            if (_selectedApplicationGroupId.HasValue)
                result += 1;

            result += SelectedApplicationFilters.Count;

            return result;
        }

        public async Task OpenExecutableSelector(int id)
        {
            var s = await DialogService.ShowExecutableSelectorDialogAsync(id);
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

        #endregion

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ApplicationsPageService.ViewState);
            this.SubscribeChange(SearchService.ViewState);

            //await ApplicationsPageService.LoadApplicationsAsync();

            List<ApplicationFilterOptionViewModel> options = new List<ApplicationFilterOptionViewModel>();
            options.Add(new ApplicationFilterOptionViewModel() { Id = 1, Name = "Free to Play" });
            options.Add(new ApplicationFilterOptionViewModel() { Id = 2, Name = "Subscription Based" });
            options.Add(new ApplicationFilterOptionViewModel() { Id = 3, Name = "Multiple" });
            options.Add(new ApplicationFilterOptionViewModel() { Id = 4, Name = "All" });

            ApplicationFilters = new List<ApplicationFilterViewModel>();
            ApplicationFilters.Add(new ApplicationFilterViewModel() { Id = 1, Name = "Access", Options = options });
            ApplicationFilters.Add(new ApplicationFilterViewModel() { Id = 2, Name = "Rating", Options = options });
            ApplicationFilters.Add(new ApplicationFilterViewModel() { Id = 3, Name = "Type", Options = options });
            ApplicationFilters.Add(new ApplicationFilterViewModel() { Id = 4, Name = "Player mode", Options = options });

            ApplicationSortOptions = new List<ApplicationSortOptionViewModel>();
            ApplicationSortOptions.Add(new ApplicationSortOptionViewModel() { Id = 1, Name = "Popularity" });
            ApplicationSortOptions.Add(new ApplicationSortOptionViewModel() { Id = 2, Name = "Add date" });
            ApplicationSortOptions.Add(new ApplicationSortOptionViewModel() { Id = 3, Name = "Title" });
            ApplicationSortOptions.Add(new ApplicationSortOptionViewModel() { Id = 4, Name = "Use" });
            ApplicationSortOptions.Add(new ApplicationSortOptionViewModel() { Id = 5, Name = "Rating" });
            ApplicationSortOptions.Add(new ApplicationSortOptionViewModel() { Id = 6, Name = "Release Date" });

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(SearchService.ViewState);
            this.UnsubscribeChange(ApplicationsPageService.ViewState);

            base.Dispose();
        }
    }
}
