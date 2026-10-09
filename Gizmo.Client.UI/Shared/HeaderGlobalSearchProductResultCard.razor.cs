using System.Globalization;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Shared
{
    public partial class HeaderGlobalSearchProductResultCard : CustomDOMComponentBase
    {
        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        private UserProductViewState _userProductViewState;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        UserProductViewStateLookupService UserProductViewStateLookupService { get; set; }

        [Inject]
        ClientServerCartViewService ClientServerCartViewService { get; set; }

        [Inject]
        ProductDetailsPageViewState ProductDetailsPageViewState { get; set; }

        [Parameter]
        public int ProductId { get; set; }

        [Parameter]
        public string Pattern { get; set; }

        [Parameter]
        public bool Best { get; set; }

        private int Minutes => _userProductViewState.ProductType == Gizmo.Web.Api.Models.ProductType.ProductTime
            ? _userProductViewState.TimeProduct?.Minutes ?? 0
            : 0;

        protected string Length => Minutes switch
        {
            <= 0 => null,
            < 60 => Minutes.ToString(CultureInfo.CurrentCulture),
            _ => (Minutes / 60m).ToString(Minutes % 60 == 0 ? "0" : "0.#", CultureInfo.CurrentCulture),
        };

        protected string Unit => GrafitLocalization.GetString(Minutes < 60 ? GrafitResourceKeys.SHELL_PACK_UNIT_MIN : GrafitResourceKeys.SHELL_PACK_UNIT_H);

        protected bool IsPointsOnly => _userProductViewState.UnitPrice == 0 && (_userProductViewState.UnitPointsPrice ?? 0) > 0;

        protected string PriceText => IsPointsOnly
            ? _userProductViewState.UnitPointsPrice.GetValueOrDefault().ToString("N0", CultureInfo.CurrentCulture)
            : _userProductViewState.UnitPrice.ToString(_userProductViewState.UnitPrice == decimal.Truncate(_userProductViewState.UnitPrice) ? "C0" : "C", CultureInfo.CurrentCulture);

        private void OnClickHandler()
        {
            if (ProductDetailsPageViewState.DisableProductDetails)
                return;

            NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + $"?ProductId={ProductId}");
        }

        private void OnClickActionButtonHandler()
        {
            ClientServerCartViewService.AddProduct(ProductId);
        }

        protected override async Task OnInitializedAsync()
        {
            _userProductViewState = await UserProductViewStateLookupService.GetStateAsync(ProductId);

            if (_userProductViewState != null)
                this.SubscribeChange(_userProductViewState);

            await base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            if (_userProductViewState != null)
                this.UnsubscribeChange(_userProductViewState);

            base.Dispose();
        }
    }
}
