using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI;
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
    [DefaultRoute("/apps"), Route("/apps"), Route("/apps/{appId:int}")]
    public partial class AppsIndex : ComponentBase
    {
        public AppsIndex()
        {
        }

        #region FIELDS
        private int? _selectedApplicationGroupId;
        #endregion

        #region PROPERTIES

        [Inject]
        ApplicationsPageService ApplicationsPageService { get; set; }

        [Inject]
        ExecutableSelectorService ExecutableSelectorService { get; set; }

        [Inject]
        AdvertisementsService AdvertisementsService { get; set; }

        [Inject]
        SearchService SearchService { get; set; }

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

        public bool ExecutableSelectorIsOpen { get; set; }

        public List<ApplicationFilterViewModel> ApplicationFilters { get; set; }

        #endregion

        #region EVENTS

        #endregion

        #region METHODS

        public IEnumerable<ApplicationViewState> GetFilteredApplications()
        {
            var result = ApplicationsPageService.ViewState.Applications.AsQueryable();
            if (SearchService.ViewState.ShowAll)
            {
                var ids = SearchService.ViewState.ApplicationResults.Select(a => a.Id).ToList();

                result = result.Where(a => ids.Contains(a.Id));
            }

            if (_selectedApplicationGroupId.HasValue)
            {
                result = result.Where(a => a.ApplicationGroupId == _selectedApplicationGroupId);
            }

            return result.ToList();
        }

        public async Task OpenExecutableSelector(int id)
        {
            await ExecutableSelectorService.LoadApplicationAsync(id);
            ExecutableSelectorIsOpen = true;
        }

        #endregion

        protected override Task OnInitializedAsync()
        {
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

            return base.OnInitializedAsync();
        }

    }
}