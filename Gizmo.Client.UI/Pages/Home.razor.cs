using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_HOME)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_HOME_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_HOME_TITLE"), ModuleDisplayOrder(0)]
    [Route("/home")]
    public partial class Home : ComponentBase
    {
        public Home()
        {
        }

        #region FIELDS
        private int _newAppsIndex = 0;
        private int _topRatedAppsIndex = 0;
        private int _mostUsedAppsIndex = 0;
        private ApplicationViewModel _selectedApplication;
        #endregion

        #region PROPERTIES

        public bool AppDetailsIsOpen { get; set; }

        public bool ExecutableSelectorIsOpen { get; set; }

        public List<ApplicationViewModel> NewApplications { get; set; }

        public List<ApplicationViewModel> TopRatedApplications { get; set; }

        public List<ApplicationViewModel> MostUsedApplications { get; set; }

        #endregion

        #region METHODS

        public void OpenDetails(int id)
        {
            _selectedApplication = NewApplications.Where(a => a.Id == id).FirstOrDefault();

            if (_selectedApplication == null)
                _selectedApplication = TopRatedApplications.Where(a => a.Id == id).FirstOrDefault();
            if (_selectedApplication == null)
                _selectedApplication = MostUsedApplications.Where(a => a.Id == id).FirstOrDefault();

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

        protected override Task OnInitializedAsync()
        {
            Random random = new Random();

            NewApplications = Enumerable.Range(1, 17).Select(i => new ApplicationViewModel()
            {
                Id = i,
                ApplicationGroupId = random.Next(1, 5),
                Name = $"BattleNet {i}",
                Description = "Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                Image = "Battle-net.png",
                Ratings = random.Next(0, 100),
                Rate = random.Next(1, 5),
                NowPlaying = random.Next(0, 100),
                Publisher = "Very Positive Inc",
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2021, 3, 12),
                Executables = new List<ExecutableViewModel>()
                {
                    new ExecutableViewModel() { Id = 1, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 2, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 3, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 4, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 5, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) }
                },
                Tags = new List<TagViewModel>()
                {
                    new TagViewModel() { Id = 1, Name = "Multiplayer" },
                    new TagViewModel() { Id = 2, Name = "Free to Play" },
                    new TagViewModel() { Id = 3, Name = "MMO" },
                    new TagViewModel() { Id = 4, Name = "Custom Tag" },
                    new TagViewModel() { Id = 5, Name = "Action" },
                    new TagViewModel() { Id = 6, Name = "Adventure" }
                }
            }).ToList();

            TopRatedApplications = Enumerable.Range(0, 25).Select(i => new ApplicationViewModel()
            {
                Id = 100 + i,
                ApplicationGroupId = random.Next(1, 5),
                Name = $"Grand Theft Auto IV {i}",
                Description = "Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                Image = "Gta-5.png",
                Ratings = random.Next(0, 100),
                Rate = random.Next(1, 5),
                NowPlaying = random.Next(0, 100),
                Publisher = "Very Positive Inc",
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2021, 3, 12),
                Executables = new List<ExecutableViewModel>()
                {
                    new ExecutableViewModel() { Id = 1, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 2, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 3, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 4, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 5, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) }
                },
                Tags = new List<TagViewModel>()
                {
                    new TagViewModel() { Id = 1, Name = "Multiplayer" },
                    new TagViewModel() { Id = 2, Name = "Free to Play" },
                    new TagViewModel() { Id = 3, Name = "MMO" },
                    new TagViewModel() { Id = 4, Name = "Custom Tag" },
                    new TagViewModel() { Id = 5, Name = "Action" },
                    new TagViewModel() { Id = 6, Name = "Adventure" }
                }
            }).ToList();

            MostUsedApplications = Enumerable.Range(0, 25).Select(i => new ApplicationViewModel()
            {
                Id = 200 + i,
                ApplicationGroupId = random.Next(1, 5),
                Name = $"Grand Theft Auto IV {i}",
                Description = "Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                Image = "Gta-5.png",
                Ratings = random.Next(0, 100),
                Rate = random.Next(1, 5),
                NowPlaying = random.Next(0, 100),
                Publisher = "Very Positive Inc",
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2021, 3, 12),
                Executables = new List<ExecutableViewModel>()
                {
                    new ExecutableViewModel() { Id = 1, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 2, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 3, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 4, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) },
                    new ExecutableViewModel() { Id = 5, Name = "Linage II Freya Hi5", Image = "https://i.postimg.cc/0yk0qNtT/app-icon.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) }
                },
                Tags = new List<TagViewModel>()
                {
                    new TagViewModel() { Id = 1, Name = "Multiplayer" },
                    new TagViewModel() { Id = 2, Name = "Free to Play" },
                    new TagViewModel() { Id = 3, Name = "MMO" },
                    new TagViewModel() { Id = 4, Name = "Custom Tag" },
                    new TagViewModel() { Id = 5, Name = "Action" },
                    new TagViewModel() { Id = 6, Name = "Adventure" }
                }
            }).ToList();

            return base.OnInitializedAsync();
        }
    }
}