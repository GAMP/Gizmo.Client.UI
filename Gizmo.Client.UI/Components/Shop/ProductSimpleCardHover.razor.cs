using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using System.Threading.Tasks;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductSimpleCardHover : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (Product != null)
            {
                this.SubscribeChange(Product);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (Product != null)
            {
                this.UnsubscribeChange(Product);
            }

            base.Dispose();
        }
    }
}
