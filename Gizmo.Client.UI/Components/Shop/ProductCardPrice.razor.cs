using System.Globalization;
using System.Linq;

using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductCardPrice : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        ClientServerCartViewService CartService { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        private decimal? CartUnitPrice
        {
            get
            {
                var line = CartService.ViewState.Products.FirstOrDefault(a =>
                    a.ProductId == Product.Id && a.Quantity > 0 && a.PayType == OrderLinePayType.Cash);

                return line is null || line.TotalPrice <= 0 ? null : line.TotalPrice / line.Quantity;
            }
        }

        protected bool HasOwnPrice => CartUnitPrice is decimal own && own < Product.UnitPrice;

        protected string OwnPriceText => CartUnitPrice.GetValueOrDefault().ToString("C", CultureInfo.CurrentCulture);

        protected string ListPriceText => Product.UnitPrice.ToString("C", CultureInfo.CurrentCulture);

        protected override void OnInitialized()
        {
            this.SubscribeChange(CartService.ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(CartService.ViewState);

            base.Dispose();
        }
    }
}
