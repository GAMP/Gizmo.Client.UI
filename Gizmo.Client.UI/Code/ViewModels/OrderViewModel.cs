using Gizmo.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.ViewModels
{
    public class OrderViewModel : ViewModelBase
    {
        private List<OrderLineViewModel> _orderLines;

        public int Id { get; set; }

        public string UserNote { get; set; }

        public decimal Total
        {
            get
            {
                return OrderLines.Sum(a => a.Total);
            }
        }

        public int PointsTotal
        {
            get
            {
                return OrderLines.Sum(a => a.PointsTotal);
            }
        }

        public int PointsAwardTotal
        {
            get
            {
                return OrderLines.Sum(a => a.PointsAwardTotal ?? 0);
            }
        }

        public List<OrderLineViewModel> OrderLines
        {
            get
            {
                if (_orderLines == null)
                    _orderLines = new List<OrderLineViewModel>();

                return _orderLines;
            }
            set
            {
                _orderLines = value;
            }
        }
    }
}