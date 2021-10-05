using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Components
{
    public partial class ExecutableSelectorDialog : CustomDOMComponentBase
    {
        public ExecutableSelectorDialog()
        {
            Random random = new Random();

            Application = new ApplicationViewModel()
            {
                Name = "Fall Guys: Ultimate Knockout",
                Description = "Fall Guys is a massively multiplayer party game with up to 60 players online in a free-for-all struggle through round after round of escalating chaos until one victor remains!",
                Image = "",
                Ratings = 168,
                Rate = 4,
                Publisher = "Very Positive Inc",
                ReleaseDate = new DateTime(2019, 10, 22),
                DateAdded = new DateTime(2020, 5, 13)
            };

            Application.Executables = new List<ExecutableViewModel>();
            Application.Executables.Add(new ExecutableViewModel() { Id = 1, Name = "Google Chrome", Image = "https://i.postimg.cc/Jhr89jNZ/chrome.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 2, Name = "Explorer", Image = "https://i.postimg.cc/fTvD5HXD/exp.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 3, Name = "My computer", Image = "https://i.postimg.cc/q74KSDDw/pc.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 4, Name = "Dota 2", Image = "https://i.postimg.cc/W120tb0j/dota2.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });
            Application.Executables.Add(new ExecutableViewModel() { Id = 5, Name = "Spotify", Image = "https://i.postimg.cc/RFjZ5NVm/spoty.png", Installing = Convert.ToBoolean(random.Next(0, 2)), InstallingPercentage = random.Next(0, 100) });
        }

        private bool _isOpen { get; set; }

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;

                _ = IsOpenChanged.InvokeAsync(_isOpen);
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        public ApplicationViewModel Application { get; set; }

        private void CloseDialog()
        {
            IsOpen = false;
        }
    }
}
