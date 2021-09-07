using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class GameStatisticsViewModel : ViewModelBase
    {
        public int Rank { get; set; }
        public int Arrow { get; set; }
        public string Flag { get; set; }
        public string Player { get; set; }
        public string Tier { get; set; }
        public int LP { get; set; }
        public int Games { get; set; }
        public decimal KDA { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
    }
}
