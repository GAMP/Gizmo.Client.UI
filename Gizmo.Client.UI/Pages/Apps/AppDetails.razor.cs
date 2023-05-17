using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.ApplicationDetailsRoute)]
    public partial class AppDetails : CustomDOMComponentBase
    {
        #region FIELDS
        private bool _showMore;
        private int _selectedTabIndex;
        #endregion

        #region PROPERTIES

        [Inject]
        AppCategoryViewStateLookupService AppCategoryViewStateLookupService { get; set; }

        [Inject]
        AppEnterpriseViewStateLookupService AppEnterpriseViewStateLookupService { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        AppDetailsPageViewService AppDetailsPageService { get; set; }

        [Inject]
        AppDetailsPageViewState ViewState { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int ApplicationId { get; set; }

        #endregion

        #region METHODS

        private Task SelectTab(int tabIndex)
        {
            _selectedTabIndex = tabIndex;

            StateHasChanged();

            return Task.CompletedTask;
        }

        #endregion

        private Task OnClickMainButtonHandler()
        {
            _showMore = true;
            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
