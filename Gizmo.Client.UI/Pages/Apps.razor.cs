﻿using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_APPS)]
    [ModuleDisplayOrder(1)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_APPS_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_APPS_DESCRIPTION")]
    [DefaultRoute("/apps"), Route("/apps"), Route("/apps/{appId:int}")]
    public partial class Apps : ComponentBase
    {
        public Apps()
        {
            Random random = new Random();

            ApplicationGroups = new List<ApplicationGroupViewModel>();
            ApplicationGroups.Add(new ApplicationGroupViewModel() { Id = 1, Name = "Adventure" });
            ApplicationGroups.Add(new ApplicationGroupViewModel() { Id = 2, Name = "Online & MMOs" });
            ApplicationGroups.Add(new ApplicationGroupViewModel() { Id = 3, Name = "FPS & Action" });
            ApplicationGroups.Add(new ApplicationGroupViewModel() { Id = 4, Name = "Strategy" });
            ApplicationGroups.Add(new ApplicationGroupViewModel() { Id = 5, Name = "Sports" });

            Applications = new List<ApplicationViewModel>();
            Applications.Add(new ApplicationViewModel() { Id = 1, Name = "Grand Theft Auto IV", Image = "Gta-5.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 2, Name = "Cyberpunk 2077", Image = "Cyber Punks.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 3, Name = "Fortnite", Image = "Fortnite.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 4, Name = "Minecraft", Image = "Minecraft.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 5, Name = "League of Legends", Image = "League.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 6, Name = "Steam", Image = "Steam.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 7, Name = "Epic", Image = "Epic-games.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 8, Name = "Valorant", Image = "Valorant.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 9, Name = "Apex Legends", Image = "Apex.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });
            Applications.Add(new ApplicationViewModel() { Id = 10, Name = "Windows Apps", Image = "Window-apps.png", ApplicationGroupId = random.Next(1, 5), Ratings = random.Next(0, 100), Rate = random.Next(1, 5), NowPlaying = random.Next(0, 100) });

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
        }

        #region FIELDS
        private ICommand _selectApplicationGroupCommand;
        private int? _selectedApplicationGroupId;
        private ApplicationGroupViewModel _selectedApplicationGroup;
        #endregion

        #region PROPERTIES

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

        public List<ApplicationViewModel> Applications { get; set; }

        public List<ApplicationFilterViewModel> ApplicationFilters { get; set; }

        #endregion

        #region COMMANDS

        public ICommand SelectApplicationGroupCommand
        {
            get
            {
                if (_selectApplicationGroupCommand == null)
                    _selectApplicationGroupCommand = new SimpleCommand<object, object>(SelectApplicationGroup);

                return _selectApplicationGroupCommand;
            }
            set
            {
                _selectApplicationGroupCommand = value;
            }
        }

        #endregion

        #region COMMAND IMPLEMENTATION

        private void SelectApplicationGroup(object parameter)
        {
            _selectedApplicationGroupId = (int)parameter;
            _selectedApplicationGroup = ApplicationGroups.Where(a => a.Id == _selectedApplicationGroupId).FirstOrDefault();

            StateHasChanged();
        }

        #endregion

        #region METHODS

        public void OpenDetails(int id)
        {
            AppDetailsIsOpen = true;
        }

        public void OpenExecutableSelector(int id)
        {
            ExecutableSelectorIsOpen = true;
        }

        #endregion

    }
}