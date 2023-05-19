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
        private IEnumerable<AppLinkViewState> _appLinkViewStates = Enumerable.Empty<AppLinkViewState>();
        private bool _hasMedia;
        private bool _hasLinks;
        #endregion

        #region PROPERTIES

        [Inject]
        NavigationService NavigationService { get; set; }

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

        [Inject]
        AppLinkViewStateLookupService AppLinkViewStateLookupService { get; set; }

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

        private Task OnClickBackButtonHandler()
        {
            return NavigationService.GoBackAsync();
        }

        private Task OnClickMainButtonHandler()
        {
            _showMore = true;
            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            this.SubscribeChange(ViewState);

            try
            {
                var appLinks = await AppLinkViewStateLookupService.GetStatesAsync();
                _appLinkViewStates = appLinks.Where(exe => exe.ApplicationId == ApplicationId);

                _hasMedia = _appLinkViewStates.Where(a => a.MediaUrlType != AdvertisementMediaUrlType.None).Any();
                _hasLinks = _appLinkViewStates.Where(a => a.MediaUrlType == AdvertisementMediaUrlType.None).Any();

                if (!_hasMedia && _hasLinks)
                {
                    _selectedTabIndex = 1;
                }
            }
            catch (Exception ex)
            {

            }

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
