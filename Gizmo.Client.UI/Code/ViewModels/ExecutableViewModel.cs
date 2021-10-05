using Gizmo.Shared.ViewModels;
using System;

namespace Gizmo.Client.UI.ViewModels
{
    public class ExecutableViewModel : ViewModelBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Image { get; set; }

        public bool Installing { get; set; }

        public decimal InstallingPercentage { get; set; }
    }
}