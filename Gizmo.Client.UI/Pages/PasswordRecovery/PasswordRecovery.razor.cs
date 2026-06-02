using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public partial class PasswordRecovery : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        PasswordRecoveryViewService PasswordRecoveryViewService { get; set; }

        [Inject]
        UserLoginViewService UserLoginService { get; set; }

        [Inject]
        PasswordRecoveryViewState ViewState { get; set; }

        [Inject]
        UserRegistrationConfigurationViewState UserRegisterConfigurationViewState { get; init; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public void OnCloseButtonClickHandler()
        {
            PasswordRecoveryViewService.Reset();
        }

        private string GetMatchValueLabel()
        {
            return string.Join(" / ", new[]
            {
                LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_EMAIL)),
                LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_USERNAME)),
                LocalizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_PHONE_NUMBER)),
            });
        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(ViewState);
            this.SubscribeChange(UserRegisterConfigurationViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(ViewState);
            this.UnsubscribeChange(UserRegisterConfigurationViewState);

            base.Dispose();
        }
    }
}
