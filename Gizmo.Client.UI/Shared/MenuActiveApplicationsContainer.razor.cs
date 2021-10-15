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
            Executables.Add(new ExecutableViewModel() { Id = 1, Name = "battlenet.exe", Category = "Gaming", Image = "_content/Gizmo.Client.UI/img/bNet.png" });
            Executables.Add(new ExecutableViewModel() { Id = 2, Name = "DOTA", Category = "Gaming", Image = "_content/Gizmo.Client.UI/img/dota-2-icon 1.png", Installing = true, InstallingPercentage = 40 });
            Executables.Add(new ExecutableViewModel() { Id = 3, Name = "Spotify", Category = "music", Image = "_content/Gizmo.Client.UI/img/spotify-512.png" });
            Executables.Add(new ExecutableViewModel() { Id = 4, Name = "valve_steamclient.exe", Category = "Gaming", Image = "_content/Gizmo.Client.UI/img/Word-2-icon 1.png" });

            Executables.Add(new ExecutableViewModel() { Id = 1, Name = "battlenet.exe", Category = "Gaming", Image = "_content/Gizmo.Client.UI/img/bNet.png" });
            Executables.Add(new ExecutableViewModel() { Id = 2, Name = "DOTA", Category = "Gaming", Image = "_content/Gizmo.Client.UI/img/dota-2-icon 1.png", Installing = true, InstallingPercentage = 40 });
            Executables.Add(new ExecutableViewModel() { Id = 3, Name = "Spotify", Category = "music", Image = "_content/Gizmo.Client.UI/img/spotify-512.png" });
            Executables.Add(new ExecutableViewModel() { Id = 4, Name = "valve_steamclient.exe", Category = "Gaming", Image = "_content/Gizmo.Client.UI/img/Word-2-icon 1.png" });
            Executables.Add(new ExecutableViewModel() { Id = 1, Name = "battlenet.exe", Category = "Gaming", Image = "_content/Gizmo.Client.UI/img/bNet.png" });
            Executables.Add(new ExecutableViewModel() { Id = 2, Name = "DOTA", Category = "Gaming", Image = "_content/Gizmo.Client.UI/img/dota-2-icon 1.png", Installing = true, InstallingPercentage = 40 });
            Executables.Add(new ExecutableViewModel() { Id = 3, Name = "Spotify", Category = "music", Image = "_content/Gizmo.Client.UI/img/spotify-512.png" });
            Executables.Add(new ExecutableViewModel() { Id = 4, Name = "valve_steamclient.exe", Category = "Gaming", Image = "_content/Gizmo.Client.UI/img/Word-2-icon 1.png" });
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