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

        protected bool IsPlainLaunch(AppExeViewState executable) =>
            ViewState.Executables.Count() == 1 &&
            (string.IsNullOrWhiteSpace(executable.Caption) ||
             string.Equals(executable.Caption.Trim(), ViewState.Application?.Title?.Trim(), StringComparison.OrdinalIgnoreCase));

        protected bool HasExecutables => ViewState.Executables.Any();

        protected bool ShowsName(AppExeViewState executable) => !IsPlainLaunch(executable);

        protected string LaunchClass(AppExeViewState executable)
        {
            if (executable != ViewState.Executables.FirstOrDefault())
                return "giz-app-page__exe";

            return IsPlainLaunch(executable)
                ? "giz-app-page__exe giz-app-page__exe--main giz-app-page__exe--play"
                : "giz-app-page__exe giz-app-page__exe--main";
        }

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

        protected int FactCount => (PublisherId.HasValue ? 1 : 0) + (ReleaseDateText.Length > 0 ? 1 : 0) + (AddDateText.Length > 0 ? 1 : 0);

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
