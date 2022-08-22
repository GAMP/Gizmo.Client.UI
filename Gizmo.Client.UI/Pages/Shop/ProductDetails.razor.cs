using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gizmo.Client.UI.Pages
{
    [Route("/productdetails/{ProductId:int}")]
    public partial class ProductDetails : ComponentBase
    {
        public ProductDetails()
        {
            Random random = new Random();

            Product = new ProductViewModel()
            {
                Id = 1,
                ProductGroupId = random.Next(1, 5),
                Name = $"Coca Cola 500ml",
                Image = "Cola.png",
                Price = random.Next(1, 5),
                PointsPrice = random.Next(0, 100),
                Points = random.Next(1, 500),
            };

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

            Order = new UserCartViewState();
        }

        [Parameter]
        public int ProductId { get; set; }

        public ProductViewModel Product { get; set; }

        public ICollection<ProductViewModel> RelatedProducts { get; set; }

        public UserCartViewState Order { get; set; }
    }
}