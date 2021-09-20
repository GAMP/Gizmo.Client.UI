using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class ApplicationViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public int ApplicationGroupId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Ratings { get; set; }
        public decimal Rate { get; set; }
        public int NowPlaying { get; set; }
    }
}