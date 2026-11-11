using System.Globalization;

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

        [Parameter]
        public UserProductViewState Product { get; set; }

        [Parameter]
        public decimal? OwnPrice { get; set; }

        protected bool HasOwnPrice => OwnPrice.HasValue;

        protected string OwnPriceText => OwnPrice.GetValueOrDefault().ToString("C", CultureInfo.CurrentCulture);

        protected string ListPriceText => Product.UnitPrice.ToString("C", CultureInfo.CurrentCulture);
    }
}
