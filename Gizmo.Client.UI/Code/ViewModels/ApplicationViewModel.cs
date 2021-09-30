using Gizmo.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace Gizmo.Client.UI.ViewModels
{
    public class ApplicationViewModel : ViewModelBase
    {
        public int Id { get; set; }

        public int ApplicationGroupId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string BackgroundImage { get; set; }

        public int Ratings { get; set; }

        public decimal Rate { get; set; }

        public int NowPlaying { get; set; }

        public string Publisher { get; set; }

        public DateTime ReleaseDate { get; set; }

        public DateTime DateAdded { get; set; }

        public List<ExecutableViewModel> Executables { get; set; }

        public List<TagViewModel> Tags { get; set; }
    }
}