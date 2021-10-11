using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductDetailsDialog : CustomDOMComponentBase
    {
        public ProductDetailsDialog()
        {
            Random random = new Random();

            Product = new ProductViewModel()
            {
                ProductGroupId = 1,
                Name = "Freddo Espresso Coffee",
                Description = "Iced coffee is a coffee beverage served cold. It may be prepared either by brewing coffee in the normal way and then serving it over ice.",
                Image = "Cola.png",
                Ratings = 168,
                Rate = 4
            };

            RelatedProducts = Enumerable.Range(0, 4).Select(i => new ProductViewModel()
            {
                Id = i,
                ProductGroupId = random.Next(1, 5),
                Name = $"Coca Cola 500ml",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
            }).ToList();
        }

        private bool _isOpen { get; set; }

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

        public ProductViewModel Product { get; set; }

        public ICollection<ProductViewModel> RelatedProducts { get; set; }

        private void CloseDialog()
        {
            IsOpen = false;
        }
    }
}
