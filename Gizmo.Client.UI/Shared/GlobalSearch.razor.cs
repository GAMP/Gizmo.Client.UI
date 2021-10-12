using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Shared
{
    public partial class GlobalSearch : CustomDOMComponentBase
    {
        public GlobalSearch()
        {
            Results = new List<SearchResultViewModel>();
            Results.Add(new SearchResultViewModel() { Type = 1, Id = 1, Name = "The Last of Us: Part II", Image = "https://i.postimg.cc/N0SGdXBy/Last-of-us.png" });
            Results.Add(new SearchResultViewModel() { Type = 2, Id = 2, Name = "Doom (2019)", Image = "https://i.postimg.cc/7PXkYRWH/doom.png" });
            Results.Add(new SearchResultViewModel() { Type = 3, Id = 3, Name = "Minecraft", Image = "https://i.postimg.cc/C1FQ3hYj/minecraft-1.png" });
            Results.Add(new SearchResultViewModel() { Type = 4, Id = 4, Name = "test", Image = "test" });
        }
        public List<SearchResultViewModel> Results { get; set; }
    }
}