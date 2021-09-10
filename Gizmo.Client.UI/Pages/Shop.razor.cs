using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_SHOP)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_SHOP_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_SHOP_DESCRIPTION"), ModuleDisplayOrder(4)]
    [Route("/shop")]
    public partial class Shop : ComponentBase
    {
        public Shop()
        {
            Random random = new Random();

            ProductCategories = new List<ProductCategoryViewModel>();
            ProductCategories.Add(new ProductCategoryViewModel() { Id = 1, Name = "Coffee", Products = 10 });
            ProductCategories.Add(new ProductCategoryViewModel() { Id = 1, Name = "Beverages", Products = 6 });
            ProductCategories.Add(new ProductCategoryViewModel() { Id = 1, Name = "FPS & Action", Products = 12 });
            ProductCategories.Add(new ProductCategoryViewModel() { Id = 1, Name = "Sandwiches", Products = 16 });
            ProductCategories.Add(new ProductCategoryViewModel() { Id = 1, Name = "Snacks", Products = 8 });
            ProductCategories.Add(new ProductCategoryViewModel() { Id = 1, Name = "Time offers", Products = 8 });

            Products = Enumerable.Range(0, 8).Select(i => new ProductViewModel()
            {
                Id = i,
                ProductGroupId = random.Next(1, 5),
                Name = $"Coca Cola 500ml",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
            }).ToList();

            Order = new OrderViewModel();

            var product = Products.Where(a => a.Id == 2).FirstOrDefault();

            OrderLineViewModel orderLine = new OrderLineViewModel()
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                UnitPointsPrice = product.PointsPrice,
                UnitPointsAward = product.Points,
                Quantity = 1
            };

            Order.OrderLines.Add(orderLine);
        }

        private List<string> _sortOptions;

        public List<ProductCategoryViewModel> ProductCategories { get; set; }

        public ICollection<ProductViewModel> Products { get; set; }

        public OrderViewModel Order { get; set; }

        public void AddProduct(int id)
        {
            var existingOrderLine = Order.OrderLines.Where(a => a.ProductId == id).FirstOrDefault();
            if (existingOrderLine != null)
            {
                existingOrderLine.Quantity += 1;
            }
            else
            {
                var product = Products.Where(a => a.Id == id).FirstOrDefault();

                OrderLineViewModel orderLine = new OrderLineViewModel()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    UnitPointsPrice = product.PointsPrice,
                    UnitPointsAward = product.Points,
                    Quantity = 1
                };

                Order.OrderLines.Add(orderLine);
            }
        }

        public void RemoveProduct(int id)
        {
            var existingOrderLine = Order.OrderLines.Where(a => a.ProductId == id).FirstOrDefault();
            if (existingOrderLine != null)
            {
                Order.OrderLines.Remove(existingOrderLine);
            }
        }

        public string SelectedSortOption { get; set; } = "Name";

        public List<string> SortOptions
        {
            get
            {
                if (_sortOptions == null)
                {
                    _sortOptions = new List<string>();
                    _sortOptions.Add("Name");
                    _sortOptions.Add("Price");
                }

                return _sortOptions;
            }
            set
            {
                _sortOptions = value;
            }
        }
    }
}