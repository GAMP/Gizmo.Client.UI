using Gizmo.Client.UI.View.States;
using Gizmo.Client.UI.ViewModels;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductCard : CustomDOMComponentBase
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Parameter]
        public bool IsHoverable { get; set; }

        [Parameter]
        public ProductViewState Product { get; set; }

        [Parameter]
        public EventCallback<int> OnAddProduct { get; set; }

        public string GetPrice()
        {
            string result = "";

            if (Product != null)
            {
                result += Product.UnitPrice.ToString("C");

                if (Product.PurchaseOptions.HasFlag(PurchaseOptionType.Or))
                {
                    result += " or " + Product.UnitPointsPrice;
                }
                else
                {
                    result += " and " + Product.UnitPointsPrice;
                }
            }

            return result;
        }

        public async Task AddProduct()
        {
            await OnAddProduct.InvokeAsync(Product.Id);
        }

        public void OpenDetails()
        {
            NavigationManager.NavigateTo($"/productdetails/{Product.Id}");
        }
    }
}