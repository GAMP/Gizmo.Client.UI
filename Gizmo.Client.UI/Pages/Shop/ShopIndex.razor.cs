using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Client.UI.Pages
{
    [ModuleGuid(KnownModules.MODULE_SHOP)]
    [PageUIModule(TitleLocalizationKey = "MODULE_PAGE_SHOP_TITLE", DescriptionLocalizationKey = "MODULE_PAGE_SHOP_DESCRIPTION"), ModuleDisplayOrder(2)]
    [Route("/shop")]
    public partial class ShopIndex : ComponentBase
    {
        public ShopIndex()
        {
            Random random = new Random();

            ProductGroups = new List<ProductGroupViewModel>();
            ProductGroups.Add(new ProductGroupViewModel() { Id = 1, Name = "Coffee" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 2, Name = "Beverages" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 3, Name = "Sandwiches" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 4, Name = "Snacks" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 5, Name = "Time offers" });

            ProductGroups.Add(new ProductGroupViewModel() { Id = 6, Name = "Coffee" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 7, Name = "Beverages" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 8, Name = "Sandwiches" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 9, Name = "Snacks" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 10, Name = "Time offers" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 11, Name = "Coffee" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 12, Name = "Beverages" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 13, Name = "Sandwiches" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 14, Name = "Snacks" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 15, Name = "Time offers" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 16, Name = "Coffee" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 17, Name = "Beverages" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 18, Name = "Sandwiches" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 19, Name = "Snacks" });
            ProductGroups.Add(new ProductGroupViewModel() { Id = 20, Name = "Time offers" });

            Products = Enumerable.Range(0, 18).Select(i => new ProductViewModel()
            {
                Id = i,
                ProductGroupId = random.Next(1, 5),
                Name = $"Coca Cola 500ml",
                Description = "Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Image = "Cola.png",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
                Ratings = random.Next(1, 500),
                Rate = random.Next(1, 5)
            }).ToList();

            Products.Add(new ProductViewModel()
            {
                ProductGroupId = 100,
                Name = "Freddo Espresso Coffee",
                Description = "Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Image = "Cola.png",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
                Ratings = random.Next(1, 500),
                Rate = random.Next(1, 5)
            });
        }

        #region FIELDS
        private List<string> _sortOptions;
        private ICommand _selectProductGroupCommand;
        private ICommand _placeOrderCommand;
        private int? _selectedProductGroupId;
        private ProductGroupViewModel _selectedProductGroup;
        #endregion

        #region PROPERTIES

        [Inject]
        UserCartService UserCartService { get; set; }

        public List<ProductGroupViewModel> ProductGroups { get; set; }

        public ProductGroupViewModel SelectedCategory { get; set; }

        public ICollection<ProductViewModel> Products { get; set; }

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

        public bool PaymentMethodSelectorIsOpen { get; set; }

        #endregion

        #region COMMANDS

        public ICommand SelectProductGroupCommand
        {
            get
            {
                if (_selectProductGroupCommand == null)
                    _selectProductGroupCommand = new AsyncCommand<object, object>(SelectProductGroup);

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
                    _placeOrderCommand = new AsyncCommand<object, object>(PlaceOrder);

                return _placeOrderCommand;
            }
            set
            {
                _placeOrderCommand = value;
            }
        }

        #endregion

        #region COMMAND IMPLEMENTATION

        private Task PlaceOrder(object parameter)
        {
            PaymentMethodSelectorIsOpen = true;

            StateHasChanged();

            return Task.CompletedTask;
        }

        private Task SelectProductGroup(object parameter)
        {
            _selectedProductGroupId = (int)parameter;
            _selectedProductGroup = ProductGroups.Where(a => a.Id == _selectedProductGroupId).FirstOrDefault();

            StateHasChanged();

            return Task.CompletedTask;
        }

        #endregion

        #region METHODS

        public Task AddProduct(int id)
        {
            return UserCartService.AddProductAsyc(id);
        }

        public void SelectPaymentMethod(int id)
        {
            StateHasChanged();
        }

        #endregion

    }
}