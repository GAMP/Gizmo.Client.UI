using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Components
{
    public partial class NewsFeedCarousel : CustomDOMComponentBase
    {
        public NewsFeedCarousel()
        {
            RssFeedItems = Enumerable.Range(0, 5).Select(i => new RssFeedItemViewModel()
            {
                Source = "game news  |  2 days ago",
                Title = "Πρώην πραγωγός του Bloodborne και της Sony Japan ιδρύει νέα εταιρεία",
                Description = "Σε αντίθεση με άλλες βιομηχανίες ψυχαγωγίας, οι φαν των βιντεοπαιχνιδιών έχουν προσκολληθεί στη νοσταλγική αγορά κουτιών, δίσκων ..."
            }).ToList();
        }

        public List<RssFeedItemViewModel> RssFeedItems { get; set; }
    }
}
