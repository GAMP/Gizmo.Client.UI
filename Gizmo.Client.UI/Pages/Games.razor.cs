using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_GAMES)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_GAMES_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_GAMES_TITLE"), ModuleDisplayOrder(2)]
    [Route("/games")]
    public partial class Games : ComponentBase
    {
        public Games()
        {
            GameStatistics = new List<GameStatisticsViewModel>();
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 01, Arrow = 1, Flag = "YCxnN8M5/Flag-1.png", Player = "ga1mingqudu", Tier = "Challenger", LP = 2354, Games = 892, KDA = 8.5m, Victories = 1000, Defeats = 400 });
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 02, Arrow = 1, Flag = "cLXq1dgt/Flag-2.png", Player = "Meng_2021", Tier = "Challenger", LP = 2354, Games = 181, KDA = 8, Victories = 1000, Defeats = 400 });
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 03, Arrow = 0, Flag = "cLXq1dgt/Flag-2.png", Player = "BLood-shennan", Tier = "Challenger", LP = 2354, Games = 282, KDA = 7.5m, Victories = 1000, Defeats = 400 });
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 04, Arrow = 0, Flag = "W4k7Wnvp/Flag-3.png", Player = "7MiuU", Tier = "Challenger", LP = 2354, Games = 453, KDA = 7.4m, Victories = 1000, Defeats = 400 });
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 05, Arrow = 0, Flag = "Hkfwyy7q/Flag-4.png", Player = "YY-TV290386", Tier = "Challenger", LP = 2354, Games = 656, KDA = 7.2m, Victories = 1000, Defeats = 400 });
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 06, Arrow = -1, Flag = "wMyxL7Nr/Flag-5.png", Player = "whnbnmkk", Tier = "Challenger", LP = 2354, Games = 353, KDA = 7, Victories = 1000, Defeats = 400 });
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 07, Arrow = 0, Flag = "YCxnN8M5/Flag-1.png", Player = "ShenJi_GuoErPiao", Tier = "Master I", LP = 1840, Games = 458, KDA = 6, Victories = 1000, Defeats = 400 });
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 08, Arrow = 1, Flag = "K8vGDqtL/Flag-6.png", Player = "Nirvana-_o", Tier = "Master I", LP = 1840, Games = 756, KDA = 6, Victories = 1000, Defeats = 400 });
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 09, Arrow = -1, Flag = "K8vGDqtL/Flag-6.png", Player = "ga1mingqudu", Tier = "Master IV", LP = 1840, Games = 654, KDA = 5.8m, Victories = 1000, Defeats = 400 });
            GameStatistics.Add(new GameStatisticsViewModel() { Rank = 10, Arrow = 1, Flag = "Hkfwyy7q/Flag-4.png", Player = "unkown1008", Tier = "Master IV", LP = 1840, Games = 325, KDA = 5.8m, Victories = 1000, Defeats = 400 });
        }

        private List<string> _filterOptions;

        public List<GameStatisticsViewModel> GameStatistics { get; set; }

        public string SelectedFilterOption { get; set; } = "Global";

        public List<string> FilterOptions
        {
            get
            {
                if (_filterOptions == null)
                {
                    _filterOptions = new List<string>();
                    _filterOptions.Add("Global");
                    _filterOptions.Add("Asia");
                    _filterOptions.Add("South America");
                    _filterOptions.Add("North America");
                    _filterOptions.Add("Europe");
                }

                return _filterOptions;
            }
            set
            {
                _filterOptions = value;
            }
        }
    }
}