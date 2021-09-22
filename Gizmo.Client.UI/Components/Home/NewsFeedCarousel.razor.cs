using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class NewsFeedCarousel : CustomDOMComponentBase
    {
        public NewsFeedCarousel()
        {
            Random random = new Random();

            NewsFeeds = new List<NewsFeed>();

            for (int i = 0; i < 6; i++)
            {
                var itemType = (Enumerations.NewsFeedType)random.Next(0, 2);

                switch (itemType)
                {
                    case Enumerations.NewsFeedType.Rss:
                        RssFeed rssItem = new RssFeed();

                        rssItem.Type = itemType;
                        rssItem.Items = Enumerable.Range(0, 5).Select(a => new RssFeedItemViewModel()
                        {
                            Source = $"game news  |  {i + 1} days ago",
                            Title = "Πρώην πραγωγός του Bloodborne και της Sony Japan ιδρύει νέα εταιρεία",
                            Description = "Σε αντίθεση με άλλες βιομηχανίες ψυχαγωγίας, οι φαν των βιντεοπαιχνιδιών έχουν προσκολληθεί στη νοσταλγική αγορά κουτιών, δίσκων ..."
                        }).ToList();

                        NewsFeeds.Add(rssItem);
                        break;

                    case Enumerations.NewsFeedType.Media:
                        MediaFeed mediaItem = new MediaFeed();

                        mediaItem.Type = itemType;
                        mediaItem.Image = "https://i.postimg.cc/50RYR6CT/news-img.png";
                        mediaItem.Title = "Το Red Bull Mobile Esports Open Season 3 επιστρέφει αυτο το καλοκαίρι";
                        mediaItem.Source = $"turnaments 2021  | {i + 1} hours ago";
                        mediaItem.Description = "Για τους συλλέκτες που αγαπούν να προσθέτουν τις τελευταίες κυκλοφορίες στο ράφι τους, αυτή η αλλαγή μπορεί να είναι τρομακτική.Αλλά η κυριαρχία της ψηφιακής αγορά θα σημαίνει απαραιτήτως το τέλος των φυσικών παιχνιδιών ή οι συλλέκτες θα δείξουν αρκετό ενδιαφέρον και θα ξοδέψουν αρκετά χρήματα για να κρατήσουν τα φυσικά μέσα ζωντανά, αν και σε μια νέα, πιο εξειδικευμένη μορφή;";

                        NewsFeeds.Add(mediaItem);
                        break;
                }
            }
        }

        private int _currentPage = 0;

        public List<NewsFeed> NewsFeeds { get; set; }

        public void SelectedIndexChangedHandler(int index)
        {
            _currentPage = index;
        }

        protected Task OnClickPreviousButtonHandler(MouseEventArgs args)
        {
            _currentPage -= 1;

            if (_currentPage < 0)
                _currentPage = NewsFeeds.Count - 1;

            return Task.CompletedTask;
        }

        protected Task OnClickNextButtonHandler(MouseEventArgs args)
        {
            _currentPage += 1;

            if (_currentPage > NewsFeeds.Count - 1)
                _currentPage = 0;

            return Task.CompletedTask;
        }
    }
}