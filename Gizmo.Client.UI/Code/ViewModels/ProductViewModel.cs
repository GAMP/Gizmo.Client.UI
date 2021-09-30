using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        public int Id { get; set; }

        public int ProductGroupId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public int? Points { get; set; }

        public int? PointsPrice { get; set; }
    }
}