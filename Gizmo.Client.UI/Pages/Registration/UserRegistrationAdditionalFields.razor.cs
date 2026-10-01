using System;
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

        [CascadingParameter]
        RegistrationCardContext Card { get; set; }

        public bool ShowCountry => RegistrationSession.RequiredUserInfo?.Country == true;
        public bool ShowAddress => RegistrationSession.RequiredUserInfo?.Address == true;
        public bool ShowCity => RegistrationSession.RequiredUserInfo?.City == true;
        public bool ShowPostCode => RegistrationSession.RequiredUserInfo?.PostCode == true;

        public string CountryLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_COUNTRY_REGION));
        public string AddressLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ADDRESS));
        public string CityLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CITY));
        public string PostCodeLabel => LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_POST_CODE));

        private const int PostCodeMaxLength = 20;

        private Task<bool> ValidatePostCodeCharacterAsync(char character)
        {
            var current = ViewState.PostCode ?? string.Empty;

            return Task.FromResult(current.Length < PostCodeMaxLength && character is >= '0' and <= '9');
        }

        public void OnCloseButtonClickHandler()
        {
            UserRegistrationAdditionalFieldsViewService.Reset();
        }

        private void OnViewStateChanged(object sender, EventArgs e) =>
            Card?.SetDraft(PlayerCardData.FromAdditional(ViewState, RegistrationSession));

        private async Task SubmitAsync()
        {
            var snapshot = PlayerCardData.FromAdditional(ViewState, RegistrationSession);

            void OnCleared(object sender, EventArgs e) => Card?.Complete(snapshot);

            RegistrationSession.Cleared += OnCleared;

            try
            {
                await UserRegistrationAdditionalFieldsViewService.SubmitAsync();
            }
            finally
            {
                RegistrationSession.Cleared -= OnCleared;
            }
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            ViewState.OnChange += OnViewStateChanged;
            OnViewStateChanged(this, EventArgs.Empty);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            ViewState.OnChange -= OnViewStateChanged;
            this.UnsubscribeChange(ViewState);

            base.Dispose();
        }
    }
}
