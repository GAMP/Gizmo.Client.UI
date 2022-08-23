using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        public int ProductGroupId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public decimal Price { get; set; }

        public int? Points { get; set; }

        public int? PointsPrice { get; set; }

        public int Ratings { get; set; }

        public decimal Rate { get; set; }
    }
}