using System.Globalization;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
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

        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        protected AppExeViewState MainExecutable => ViewState.Executables.FirstOrDefault();

        protected bool HasMoreExecutables => ViewState.Executables.Skip(1).Any();

        protected bool HasDescription => !string.IsNullOrWhiteSpace(ViewState.Application?.Description);

        protected int? PublisherId => ViewState.Application?.PublisherId;

        protected string ReleaseDateText => ViewState.Application?.ReleaseDate is DateTime released
            ? released.ToString("d", CultureInfo.CurrentCulture)
            : string.Empty;

        protected string AddDateText => ViewState.Application is { } app && app.AddDate.Year > 1
            ? app.AddDate.ToString("d", CultureInfo.CurrentCulture)
            : string.Empty;

        protected string LaunchLabel =>
            GrafitLocalization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_LAUNCH)).Replace("\\", "\\\\").Replace("'", "\\'");

        protected bool HasFacts => PublisherId.HasValue || ReleaseDateText.Length > 0 || AddDateText.Length > 0;

        #endregion

        private Task OnClickBackButtonHandler()
        {
            return NavigationService.GoBackAsync();
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
