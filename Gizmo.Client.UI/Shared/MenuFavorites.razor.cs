using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuFavorites : CustomDOMComponentBase
    {
        public MenuFavorites()
        {
            Favorites = new List<FavoriteViewModel>();
            Favorites.Add(new FavoriteViewModel() { Id = 1, Name = "Explorer", Icon = "https://i.postimg.cc/J4RGXr13/Places-folder-red-icon-1.png" });
            Favorites.Add(new FavoriteViewModel() { Id = 2, Name = "Word", Icon = "https://i.postimg.cc/XYG3Gmyc/Word-2-icon-1.png" });
            Favorites.Add(new FavoriteViewModel() { Id = 3, Name = "DOTA", Icon = "https://i.postimg.cc/bYm9dBPv/dota-2-icon-1.png" });
            Favorites.Add(new FavoriteViewModel() { Id = 4, Name = "Spotify", Icon = "https://i.postimg.cc/VNmGQb1r/spotify-512.png" });
            Favorites.Add(new FavoriteViewModel() { Id = 5, Name = "BattleNet", Icon = "https://i.postimg.cc/rpfvf9WF/bNet.png" });
        }

        public List<FavoriteViewModel> Favorites { get; set; }
    }
}