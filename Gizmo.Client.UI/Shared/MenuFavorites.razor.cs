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
            Favorites.Add(new FavoriteViewModel() { Id = 1, Name = "Explorer", Icon = "_content/Gizmo.Client.UI/img/Places-folder-red-icon 1.png" });
            Favorites.Add(new FavoriteViewModel() { Id = 2, Name = "Word", Icon = "_content/Gizmo.Client.UI/img/Word-2-icon 1.png" });
            Favorites.Add(new FavoriteViewModel() { Id = 3, Name = "DOTA", Icon = "_content/Gizmo.Client.UI/img/dota-2-icon 1.png" });
            Favorites.Add(new FavoriteViewModel() { Id = 4, Name = "Spotify", Icon = "_content/Gizmo.Client.UI/img/spotify-512.png" });
            Favorites.Add(new FavoriteViewModel() { Id = 5, Name = "BattleNet", Icon = "_content/Gizmo.Client.UI/img/bNet.png" });
        }

        public List<FavoriteViewModel> Favorites { get; set; }
    }
}