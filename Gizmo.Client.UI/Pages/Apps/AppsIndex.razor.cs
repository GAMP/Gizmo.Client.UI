using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

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
        private ICommand _selectApplicationGroupCommand;
        private int? _clickedApplicationGroupId;
        private int? _selectedApplicationGroupId;
        private ApplicationGroupViewModel _selectedApplicationGroup;
        private bool _selectedGroupHasSubGroups;
        private bool _clickedGroupHasSubGroups;
        #endregion

        #region PROPERTIES

        [Inject]
        ApplicationsPageService ApplicationsPageService { get; set; }

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

        public bool AppDetailsIsOpen { get; set; }

        public bool ExecutableSelectorIsOpen { get; set; }

        #endregion

        public List<ApplicationGroupViewModel> ApplicationGroups { get; set; }

        public List<ApplicationFilterViewModel> ApplicationFilters { get; set; }

        #endregion

        #region COMMANDS

        public ICommand SelectApplicationGroupCommand
        {
            get
            {
                if (_selectApplicationGroupCommand == null)
                    _selectApplicationGroupCommand = new AsyncCommand<object, object>(SelectApplicationGroup);

                return _selectApplicationGroupCommand;
            }
            set
            {
                _selectApplicationGroupCommand = value;
            }
        }

        #endregion

        #region COMMAND IMPLEMENTATION

        private Task SelectApplicationGroup(object parameter)
        {
            _clickedApplicationGroupId = (int)parameter;

            _clickedGroupHasSubGroups = ApplicationGroups.Where(a => a.ParentGroupId == _clickedApplicationGroupId).Count() > 0;

            //If the clicked application group has subgroups then set it as selected to show the subgroups.
            if (_clickedGroupHasSubGroups)
            {
                _selectedGroupHasSubGroups = _clickedGroupHasSubGroups;
                _selectedApplicationGroupId = _clickedApplicationGroupId;
                _selectedApplicationGroup = ApplicationGroups.Where(a => a.Id == _selectedApplicationGroupId).FirstOrDefault();
            }

            StateHasChanged();

            return Task.CompletedTask;
        }

        #endregion

        #region EVENTS

        protected Task OnClickBackButtonHandler(MouseEventArgs args)
        {
            _selectedApplicationGroupId = _selectedApplicationGroup.ParentGroupId;
            _selectedApplicationGroup = ApplicationGroups.Where(a => a.Id == _selectedApplicationGroupId).FirstOrDefault();

            return Task.CompletedTask;
        }

        #endregion

        #region METHODS

        public void OpenExecutableSelector(int id)
        {
            ExecutableSelectorIsOpen = true;
        }

        #endregion

        protected override Task OnInitializedAsync()
        {
            Random random = new Random();

            ApplicationGroups = new List<ApplicationGroupViewModel>();

            var category1 = new ApplicationGroupViewModel() { Id = 1, Name = "Adventure" };

            ApplicationGroups.Add(category1);
            ApplicationGroups.Add(new ApplicationGroupViewModel() { Id = 2, Name = "Online & MMOs" });
            ApplicationGroups.Add(new ApplicationGroupViewModel() { Id = 3, Name = "FPS & Action" });
            ApplicationGroups.Add(new ApplicationGroupViewModel() { Id = 4, Name = "Strategy" });
            ApplicationGroups.Add(new ApplicationGroupViewModel() { Id = 5, Name = "Sports" });

            ApplicationGroups.AddRange(Enumerable.Range(1, 8).Select(i => new ApplicationGroupViewModel()
            {
                Id = 10 + i,
                ParentGroupId = 1,
                Name = $"Subcategory {i}",
            }).ToList());

            ApplicationGroups.AddRange(Enumerable.Range(1, 8).Select(i => new ApplicationGroupViewModel()
            {
                Id = 20 + i,
                ParentGroupId = 2,
                Name = $"Subcategory2 {i}",
            }).ToList());

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

            _clickedApplicationGroupId = category1.Id;

            return base.OnInitializedAsync();
        }

    }
}