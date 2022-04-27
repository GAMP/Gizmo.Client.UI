using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Components
{
    public partial class NewsFeedCarousel : CustomDOMComponentBase
    {
        public NewsFeedCarousel()
        {
            Random random = new Random();

            NewsFeeds = new List<NewsFeedViewModel>();

            for (int i = 0; i < 6; i++)
            {
                var itemType = (NewsFeedTypes)random.Next(0, 2);

                switch (itemType)
                {
                    case NewsFeedTypes.Rss:
                        RssFeedViewModel rssItem = new RssFeedViewModel();

                        rssItem.Type = itemType;
                        rssItem.Items = Enumerable.Range(0, 5).Select(a => new RssFeedItemViewModel()
                        {
                            Source = $"game news  |  {i + 1} days ago",
                            Title = "Πρώην πραγωγός του Bloodborne και της Sony Japan ιδρύει νέα εταιρεία",
                            Description = "Σε αντίθεση με άλλες βιομηχανίες ψυχαγωγίας, οι φαν των βιντεοπαιχνιδιών έχουν προσκολληθεί στη νοσταλγική αγορά κουτιών, δίσκων ..."
                        }).ToList();

                        NewsFeeds.Add(rssItem);
                        break;

                    case NewsFeedTypes.Media:
                        MediaFeedViewModel mediaItem = new MediaFeedViewModel();

                        mediaItem.Type = itemType;
                        mediaItem.Thumb = "_content/Gizmo.Client.UI/img/Redbull.png";
                        mediaItem.Image = "_content/Gizmo.Client.UI/img/news-modal-img.png";
                        mediaItem.Title = "Το Red Bull Mobile Esports Open Season 3 επιστρέφει αυτο το καλοκαίρι";
                        mediaItem.Source = $"turnaments 2021  | {i + 1} hours ago";
                        mediaItem.Description = "Για τους συλλέκτες που αγαπούν να προσθέτουν τις τελευταίες κυκλοφορίες στο ράφι τους, αυτή η αλλαγή μπορεί να είναι τρομακτική.Αλλά η κυριαρχία της ψηφιακής αγορά θα σημαίνει απαραιτήτως το τέλος των φυσικών παιχνιδιών ή οι συλλέκτες θα δείξουν αρκετό ενδιαφέρον και θα ξοδέψουν αρκετά χρήματα για να κρατήσουν τα φυσικά μέσα ζωντανά, αν και σε μια νέα, πιο εξειδικευμένη μορφή;";

                        NewsFeeds.Add(mediaItem);
                        break;
                }
            }
        }

        public List<NewsFeedViewModel> NewsFeeds { get; set; }

    }
}