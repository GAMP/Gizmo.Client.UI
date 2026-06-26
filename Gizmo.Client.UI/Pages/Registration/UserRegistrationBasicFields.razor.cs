using System.Threading.Tasks;
using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationBasicFieldsRoute)]
    public partial class UserRegistrationBasicFields : CustomDOMComponentBase
    {
        private string? _selectedPhoneCountryName;

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewService UserRegistrationBasicFieldsViewService { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public bool HasAdditionalFields =>
            RegistrationSession.RequiredUserInfo?.Country == true ||
            RegistrationSession.RequiredUserInfo?.Address == true ||
            RegistrationSession.RequiredUserInfo?.City == true ||
            RegistrationSession.RequiredUserInfo?.PostCode == true;

        public bool ShowPassword => true;
        public bool ShowFirstName => RegistrationSession.RequiredUserInfo?.FirstName == true;
        public bool ShowLastName => RegistrationSession.RequiredUserInfo?.LastName == true;
        public bool ShowBirthDate => RegistrationSession.RequiredUserInfo?.BirthDate == true;
        public bool ShowSex => RegistrationSession.RequiredUserInfo?.Sex == true;
        public bool ShowEmail => RegistrationSession.Flow != RegistrationFlow.Email && RegistrationSession.RequiredUserInfo?.Email == true;
        public bool ShowMobilePhone => RegistrationSession.Flow != RegistrationFlow.Sms && RegistrationSession.RequiredUserInfo?.Mobile == true;
        public bool ShowPhone => RegistrationSession.RequiredUserInfo?.Phone == true;

        public string FirstNameLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_FIRST_NAME));
        public string LastNameLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LAST_NAME));
        public string BirthDateLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_BIRTH_DATE));
        public string SexLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_GENDER));
        public string EmailLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_EMAIL_ADDRESS));
        public string MobilePhoneLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_MOBILE_PHONE));
        public string PhoneLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_PHONE));

        private Task OnPhoneCountryChangedAsync(PhoneCountrySelection? selection)
        {
            _selectedPhoneCountryName = selection?.CountryName;

            if (selection == null)
            {
                UserRegistrationBasicFieldsViewService.SetPhoneRegionCode(null);
                UserRegistrationBasicFieldsViewService.SetMobilePhone(null);
            }
            else
            {
                UserRegistrationBasicFieldsViewService.SetPhoneRegionCode(selection.RegionCode);
                UserRegistrationBasicFieldsViewService.SetMobilePhone(selection.CallingCodeDigits);
            }
            return Task.CompletedTask;
        }

        private Task OnPhoneValueChangedAsync(string? value)
        {
            UserRegistrationBasicFieldsViewService.SetMobilePhone(value);
            return Task.CompletedTask;
        }

        public void OnCloseButtonClickHandler()
        {
            UserRegistrationBasicFieldsViewService.Reset();
        }

        private Task NavigateBackAsync()
        {
            RegistrationSession.Clear();

            var route = RegistrationSession.Flow switch
            {
                RegistrationFlow.Email => ClientRoutes.RegistrationEmailRoute,
                RegistrationFlow.Sms   => ClientRoutes.RegistrationPhoneRoute,
                _                      => ClientRoutes.RegistrationProvidersRoute
            };

            NavigationService.NavigateTo(route);
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
