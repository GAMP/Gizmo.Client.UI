using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Gizmo.Client.UI.Pages
{
    [Route("/profile/history")]
    public partial class History : ComponentBase
    {
        public History()
        {
            GameStatistics = new List<PurchaseViewModel>();
            GameStatistics.Add(new PurchaseViewModel() { ProductName = "Espresso Coffee", Quantity = 1, Date = new DateTime(2020, 1, 2), Total = 2.50m, PaymentMethod = "Balance" });
            GameStatistics.Add(new PurchaseViewModel() { ProductName = "Redbull 330ml", Quantity = 2, Date = new DateTime(2020, 5, 12), Total = 4.59m, PaymentMethod = "Credit Card" });
            GameStatistics.Add(new PurchaseViewModel() { ProductName = "Panini Turkey Croissant", Quantity = 1, Date = new DateTime(2020, 5, 16), Total = 1.99m, PaymentMethod = "Credit Card" });
            GameStatistics.Add(new PurchaseViewModel() { ProductName = "Espresso Coffee", Quantity = 2, Date = new DateTime(2020, 7, 28), Total = 5.00m, PaymentMethod = "Balance" });
            GameStatistics.Add(new PurchaseViewModel() { ProductName = "Espresso Coffee", Quantity = 1, Date = new DateTime(2020, 7, 30), Total = 2.50m, PaymentMethod = "Balance" });
            GameStatistics.Add(new PurchaseViewModel() { ProductName = "Espresso Coffee", Quantity = 1, Date = new DateTime(2020, 8, 8), Total = 2.50m, PaymentMethod = "Balance" });
            GameStatistics.Add(new PurchaseViewModel() { ProductName = "Service: Printing", Quantity = 1, Date = new DateTime(2020, 10, 19), Total = 4.99m, PaymentMethod = "Credit Card" });
            GameStatistics.Add(new PurchaseViewModel() { ProductName = "Espresso Coffee", Quantity = 1, Date = new DateTime(2020, 10, 20), Total = 2.50m, PaymentMethod = "Balance" });
            GameStatistics.Add(new PurchaseViewModel() { ProductName = "Fredo Espresso Coffe", Quantity = 1, Date = new DateTime(2020, 11, 2), Total = 3.20m, PaymentMethod = "Balance" });
        }

        public List<PurchaseViewModel> GameStatistics { get; set; }
    }
}