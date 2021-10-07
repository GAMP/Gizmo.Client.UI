using Gizmo.Client.UI.ViewModels;
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

            NewApplications = Enumerable.Range(1, 17).Select(i => new ApplicationViewModel()
            {
                Id = i,
                ApplicationGroupId = random.Next(1, 5),
                Name = $"BattleNet {i}",
                Image = "Battle-net.png",
                Ratings = random.Next(0, 100),
                Rate = random.Next(1, 5),
                NowPlaying = random.Next(0, 100),
                DateAdded = new DateTime(2021, 3, 12)
            }).ToList();

            Applications = Enumerable.Range(0, 25).Select(i => new ApplicationViewModel()
            {
                Id = i,
                ApplicationGroupId = random.Next(1, 5),
                Name = $"Grand Theft Auto IV {i}",
                Image = "Gta-5.png",
                Ratings = random.Next(0, 100),
                Rate = random.Next(1, 5),
                NowPlaying = random.Next(0, 100),
            }).ToList();
        }

        private int _newAppsIndex = 0;

        private int _appsIndex = 0;


        public bool AppDetailsIsOpen { get; set; }

        public bool ExecutableSelectorIsOpen { get; set; }

        public List<ApplicationViewModel> NewApplications { get; set; }

        public List<ApplicationViewModel> Applications { get; set; }

        #region METHODS

        public void OpenDetails(int id)
        {
            AppDetailsIsOpen = true;
        }

        public void OpenExecutableSelector(int id)
        {
            ExecutableSelectorIsOpen = true;
        }

        private int GetPages(int items, int divider)
        {
            int pages = items / divider;
            if (items % divider != 0)
            {
                pages += 1;
            }
            return pages;
        }

        #endregion

    }
}
