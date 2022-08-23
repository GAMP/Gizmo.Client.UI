using Gizmo.Shared.ViewModels;
using System;

namespace Gizmo.Client.UI.ViewModels
{
    public class PurchaseViewModel
    {
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public DateTime Date { get; set; }

        public decimal Total { get; set; }

        public string PaymentMethod { get; set; }
    }
}