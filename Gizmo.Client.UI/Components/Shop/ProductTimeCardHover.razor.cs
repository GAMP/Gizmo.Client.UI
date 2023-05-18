using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductTimeCardHover : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public UserProductTimeViewState TimeProduct { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (TimeProduct != null)
            {
                this.SubscribeChange(TimeProduct);
            }

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (TimeProduct != null)
            {
                this.UnsubscribeChange(TimeProduct);
            }

            base.Dispose();
        }
    }
}
