﻿using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components.Infrastructure;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

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

            ProductGroups = new List<ProductGroupViewModel>();
            ProductGroups.Add(new ProductGroupViewModel() { Id = 1, Name = "Coffee" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 2, Name = "Beverages" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 3, Name = "Sandwiches" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 4, Name = "Snacks" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 5, Name = "Time offers" });

            Products = Enumerable.Range(0, 18).Select(i => new ProductViewModel()
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

        #region FIELDS
        private List<string> _sortOptions;
        private ICommand _selectProductGroupCommand;
        private ICommand _placeOrderCommand;
        private int? _selectedProductGroup;
        #endregion

        #region PROPERTIES

        public List<ProductGroupViewModel> ProductGroups { get; set; }

        public ProductGroupViewModel SelectedCategory { get; set; }

        public ICollection<ProductViewModel> Products { get; set; }

        public OrderViewModel Order { get; set; }

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

        public bool SelectPaymentMethodIsOpen { get; set; }

        public bool ProductDetailsIsOpen { get; set; }

        #endregion

        #region COMMANDS

        public ICommand SelectProductGroupCommand
        {
            get
            {
                if (_selectProductGroupCommand == null)
                    _selectProductGroupCommand = new SimpleCommand<object, object>(SelectProductGroup);

                return _selectProductGroupCommand;
            }
            set
            {
                _selectProductGroupCommand = value;
            }
        }

        public ICommand PlaceOrderCommand
        {
            get
            {
                if (_placeOrderCommand == null)
                    _placeOrderCommand = new SimpleCommand<object, object>(PlaceOrder);

                return _placeOrderCommand;
            }
            set
            {
                _placeOrderCommand = value;
            }
        }

        #endregion

        #region COMMAND IMPLEMENTATION

        private void PlaceOrder(object parameter)
        {
            SelectPaymentMethodIsOpen = true;

            StateHasChanged();
        }

        private void SelectProductGroup(object parameter)
        {
            _selectedProductGroup = (int)parameter;

            StateHasChanged();
        }

        #endregion

        #region METHODS

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

        public void OpenDetails(int id)
        {
            ProductDetailsIsOpen = true;
        }

        #endregion

    }
}