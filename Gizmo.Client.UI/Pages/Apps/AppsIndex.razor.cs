﻿using Gizmo.Client.UI.Components;
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
    [DefaultRoute("/apps"), Route("/apps"), Route("/apps/{appId:int}")]
    public partial class AppsIndex : CustomDOMComponentBase
    {
        public AppsIndex()
        {
        }

        #region FIELDS
        private int? _selectedSortOptionId;
        private int? _selectedApplicationGroupId;
        private List<int> _selectedApplicationFilters = new List<int>() { 1, 3 };
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
            await ExecutableSelectorService.LoadApplicationAsync(id);

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

        #endregion

        protected override Task OnInitializedAsync()
        {
            this.SubscribeChange(SearchService.ViewState);

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
            ApplicationSortOptions.Add(new ApplicationSortOptionViewModel() { Id = 1, Name = "Default" });
            ApplicationSortOptions.Add(new ApplicationSortOptionViewModel() { Id = 2, Name = "Test 1" });
            ApplicationSortOptions.Add(new ApplicationSortOptionViewModel() { Id = 3, Name = "Test 2" });

            return base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(SearchService.ViewState);
            base.Dispose();
        }
    }
}