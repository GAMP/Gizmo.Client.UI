using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuActiveApplicationsContainer : CustomDOMComponentBase
    {
        public MenuActiveApplicationsContainer()
        {
            Executables = new List<ExecutableViewModel>();
            Executables.Add(new ExecutableViewModel() { Id = 1, Name = "battlenet.exe", Category = "Gaming", Image = "https://i.postimg.cc/B64jDChP/Battlenet.png" });
            Executables.Add(new ExecutableViewModel() { Id = 2, Name = "Discord", Category = "Gaming", Image = "https://i.postimg.cc/SsSKjxsW/Discord.png", Installing = true, InstallingPercentage = 40 });
            Executables.Add(new ExecutableViewModel() { Id = 3, Name = "Spotify", Category = "music", Image = "https://i.postimg.cc/s2vrLGvH/Spotify.png" });
            Executables.Add(new ExecutableViewModel() { Id = 4, Name = "valve_steamclient.exe", Category = "Gaming", Image = "https://i.postimg.cc/zfZYQ5cZ/App-Icon.png" });

            Executables.Add(new ExecutableViewModel() { Id = 1, Name = "battlenet.exe", Category = "Gaming", Image = "https://i.postimg.cc/B64jDChP/Battlenet.png" });
            Executables.Add(new ExecutableViewModel() { Id = 2, Name = "Discord", Category = "Gaming", Image = "https://i.postimg.cc/SsSKjxsW/Discord.png", Installing = true, InstallingPercentage = 10 });
            Executables.Add(new ExecutableViewModel() { Id = 3, Name = "Spotify", Category = "music", Image = "https://i.postimg.cc/s2vrLGvH/Spotify.png" });
            Executables.Add(new ExecutableViewModel() { Id = 4, Name = "valve_steamclient.exe", Category = "Gaming", Image = "https://i.postimg.cc/zfZYQ5cZ/App-Icon.png" });
            Executables.Add(new ExecutableViewModel() { Id = 1, Name = "battlenet.exe", Category = "Gaming", Image = "https://i.postimg.cc/B64jDChP/Battlenet.png" });
            Executables.Add(new ExecutableViewModel() { Id = 2, Name = "Discord", Category = "Gaming", Image = "https://i.postimg.cc/SsSKjxsW/Discord.png", Installing = true, InstallingPercentage = 30 });
            Executables.Add(new ExecutableViewModel() { Id = 3, Name = "Spotify", Category = "music", Image = "https://i.postimg.cc/s2vrLGvH/Spotify.png" });
            Executables.Add(new ExecutableViewModel() { Id = 4, Name = "valve_steamclient.exe", Category = "Gaming", Image = "https://i.postimg.cc/zfZYQ5cZ/App-Icon.png" });
            Executables.Add(new ExecutableViewModel() { Id = 1, Name = "battlenet.exe", Category = "Gaming", Image = "https://i.postimg.cc/B64jDChP/Battlenet.png" });
            Executables.Add(new ExecutableViewModel() { Id = 2, Name = "Discord", Category = "Gaming", Image = "https://i.postimg.cc/SsSKjxsW/Discord.png", Installing = true, InstallingPercentage = 90 });
            Executables.Add(new ExecutableViewModel() { Id = 3, Name = "Spotify", Category = "music", Image = "https://i.postimg.cc/s2vrLGvH/Spotify.png" });
            Executables.Add(new ExecutableViewModel() { Id = 4, Name = "valve_steamclient.exe", Category = "Gaming", Image = "https://i.postimg.cc/zfZYQ5cZ/App-Icon.png" });
        }

        private bool _isOpen { get; set; }

        public List<ExecutableViewModel> Executables { get; set; }

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

    }
}