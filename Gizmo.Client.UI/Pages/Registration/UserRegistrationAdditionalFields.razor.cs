using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationAdditionalFieldsRoute)]
    public partial class UserRegistrationAdditionalFields : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        UserRegistrationAdditionalFieldsViewService UserRegistrationAdditionalFieldsViewService { get; set; }

        [Inject]
        UserRegistrationAdditionalFieldsViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public bool ShowCountry => RegistrationSession.RequiredUserInfo?.Country == true;
        public bool ShowAddress => RegistrationSession.RequiredUserInfo?.Address == true;
        public bool ShowCity => RegistrationSession.RequiredUserInfo?.City == true;
        public bool ShowPostCode => RegistrationSession.RequiredUserInfo?.PostCode == true;

        public string CountryLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_COUNTRY_REGION));
        public string AddressLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ADDRESS));
        public string CityLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CITY));
        public string PostCodeLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_POST_CODE));

        public void OnCloseButtonClickHandler()
        {
            UserRegistrationAdditionalFieldsViewService.Reset();
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
