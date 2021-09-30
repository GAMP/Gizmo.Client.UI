using Gizmo.Shared.ViewModels;

namespace Gizmo.Client.UI.ViewModels
{
    public class OrderLineViewModel : ViewModelBase
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal Quantity { get; set; }

        public decimal DeliveredQuantity { get; set; }

        public decimal UnitPrice { get; set; }

        public int? UnitPointsPrice { get; set; }

        public int? UnitPointsAward { get; set; }

        public decimal Tax { get; set; }

        public decimal Total
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }

        public int PointsTotal { get; set; }

        public int? PointsAwardTotal { get; set; }

        public decimal TaxTotal { get; set; }
    }
}