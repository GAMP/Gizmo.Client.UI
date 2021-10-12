namespace Gizmo.Client.UI.ViewModels
{
    public class SearchResultViewModel
    {
        public int Type { get; set; } //1 = App, 2 = Product

        public int Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }
    }
}
