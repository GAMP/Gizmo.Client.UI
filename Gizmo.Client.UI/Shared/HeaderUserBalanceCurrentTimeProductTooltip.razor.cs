using System.Threading.Tasks;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class HeaderUserBalanceCurrentTimeProductTooltip : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        [Inject]
        TimeProductsViewState TimeProductsViewState { get; set; }

        [Inject]
        CreditOptionsViewState CreditOptionsViewState { get; set; }

        public void OpenDetails()
        {
            NavigationService.NavigateTo(ClientRoutes.UserProductsRoute);
        }

        #region OVERRIDE

        protected override Task OnInitializedAsync()
        {
            this.SubscribeChange(TimeProductsViewState);

            return base.OnInitializedAsync();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(TimeProductsViewState);

            base.Dispose();
        }

        #endregion
    }
}
