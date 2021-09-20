﻿using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_HOME)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_HOME_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_HOME_TITLE"),ModuleDisplayOrder(0)]
    [Route("/home")]
    public partial class Home : ComponentBase
    {
        public Home()
        {
            Random random = new Random();

            Applications = Enumerable.Range(0, 5).Select(i => new ApplicationViewModel()
            {
                Id = i,
                ApplicationGroupId = random.Next(1, 5),
                Name = "Grand Theft Auto IV",
                Image = "Gta-5.png",
                Ratings = random.Next(0, 100),
                Rate = random.Next(1, 5),
                NowPlaying = random.Next(0, 100),
            }).ToList();
        }

        public bool AppDetailsIsOpen { get; set; }

        public List<ApplicationViewModel> Applications { get; set; }

        #region METHODS

        public void OpenDetails(int id)
        {
            AppDetailsIsOpen = true;
        }

        #endregion

    }
}
