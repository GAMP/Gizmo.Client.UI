using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class ProductBundleCardHover : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public UserProductViewState Product { get; set; }

        #region OVERRIDES

        protected override void OnInitialized()
        {
            this.SubscribeChange(Product);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(Product);

            base.Dispose();
        }

        #endregion
    }
}
