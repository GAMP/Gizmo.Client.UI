using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Pages
{
    [Route("/appdetails/{ApplicationId:int}")]
    public partial class AppDetails : ComponentBase
    {
        public AppDetails()
        {
            Random random = new Random();

            Application = new ApplicationViewModel()
            {
                Id = 1,
                Name = "Grand Theft Auto IV",
                Image = "Gta-5.png",
                ApplicationGroupId = random.Next(1, 5),
                Ratings = random.Next(0, 100),
                Rate = random.Next(1, 5),
                NowPlaying = random.Next(0, 100),
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            };

            Application.Executables = new List<ExecutableViewModel>();
            Application.Executables.Add(new ExecutableViewModel() { Id = 1, Name = "Linage II Freya Hi5", Image = "_content/Gizmo.Client.UI/img/dota-2-icon 1.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 2, Name = "Linage II Freya Hi5", Image = "_content/Gizmo.Client.UI/img/dota-2-icon 1.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 3, Name = "Linage II Freya Hi5", Image = "_content/Gizmo.Client.UI/img/dota-2-icon 1.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 4, Name = "Linage II Freya Hi5", Image = "_content/Gizmo.Client.UI/img/dota-2-icon 1.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 5, Name = "Linage II Freya Hi5", Image = "_content/Gizmo.Client.UI/img/dota-2-icon 1.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });

        }

        [Parameter]
        public int ApplicationId { get; set; }

        public ApplicationViewModel Application { get; set; }
    }
}