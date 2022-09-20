using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Shared
{
    public partial class _Layout_Login : LayoutComponentBase
    {
        public _Layout_Login()
        {
            List<string> images = new List<string>()
            {
                "_content/Gizmo.Client.UI/img/Redbull.png",
                "_content/Gizmo.Client.UI/img/carousel_1.jpg",
                "_content/Gizmo.Client.UI/img/carousel_2.jpg",
                "_content/Gizmo.Client.UI/img/carousel_3.jpg"
            };

            Random random = new Random();

            NewsFeeds = new List<NewsFeedViewModel>();

            for (int i = 0; i < 6; i++)
            {
                MediaFeedViewModel mediaItem = new MediaFeedViewModel();

                mediaItem.Type = NewsFeedTypes.Media;
                mediaItem.Thumb = images[random.Next(0, 3)];
                mediaItem.Title = "Το Red Bull Mobile Esports Open Season 3 επιστρέφει αυτο το καλοκαίρι";
                mediaItem.Source = $"turnaments 2021  | {i + 1} hours ago";
                mediaItem.Description = "Για τους συλλέκτες που αγαπούν να προσθέτουν τις τελευταίες κυκλοφορίες στο ράφι τους, αυτή η αλλαγή μπορεί να είναι τρομακτική.Αλλά η κυριαρχία της ψηφιακής αγορά θα σημαίνει απαραιτήτως το τέλος των φυσικών παιχνιδιών ή οι συλλέκτες θα δείξουν αρκετό ενδιαφέρον και θα ξοδέψουν αρκετά χρήματα για να κρατήσουν τα φυσικά μέσα ζωντανά, αν και σε μια νέα, πιο εξειδικευμένη μορφή;";

                NewsFeeds.Add(mediaItem);
            }
        }

        public List<NewsFeedViewModel> NewsFeeds { get; set; }
    }
}