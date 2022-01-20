using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductDetailsDialog : CustomDOMComponentBase
    {
        public ProductDetailsDialog()
        {
            Random random = new Random();

            RelatedProducts = Enumerable.Range(0, 4).Select(i => new ProductViewModel()
            {
                Id = i,
                ProductGroupId = random.Next(1, 5),
                Name = $"Coca Cola 500ml",
                Image = "Cola.png",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
            }).ToList();
        }

        private bool _isOpen { get; set; }
        private ICommand _addToCartCommand;

        [Parameter]
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (_isOpen == value)
                    return;

                _isOpen = value;

                _ = IsOpenChanged.InvokeAsync(_isOpen);
            }
        }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public ProductViewModel Product { get; set; }

        [Parameter]
        public EventCallback<int> OnAddToCart { get; set; }

        public ICollection<ProductViewModel> RelatedProducts { get; set; }

        #region COMMANDS

        public ICommand AddToCartCommand
        {
            get
            {
                if (_addToCartCommand == null)
                    _addToCartCommand = new AsyncCommand<object, object>(AddToCart);

                return _addToCartCommand;
            }
            set
            {
                _addToCartCommand = value;
            }
        }

        #endregion

        #region COMMAND IMPLEMENTATION

        private Task AddToCart(object parameter)
        {
            if (Product != null)
            {
                OnAddToCart.InvokeAsync(Product.Id);
                IsOpen = false;
            }

            return Task.CompletedTask;
        }

        #endregion

        private void CloseDialog()
        {
            IsOpen = false;
        }
    }
}
