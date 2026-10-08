using Gizmo.Client.UI.Components;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using System.Threading.Tasks;
using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages.Registration
{
    [Route(ClientRoutes.RegistrationConfirmationRoute)]
    public partial class UserRegistrationConfirmation : CustomDOMComponentBase
    {
        [CascadingParameter]
        protected GrafitLocalizationService GrafitLocalization { get; set; }

        [CascadingParameter]
        RegistrationCardContext Card { get; set; }

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        protected string TimerText => $"{ViewState.SecondsLeft / 60}:{ViewState.SecondsLeft % 60:D2}";

        [Inject]
        UserRegistrationConfirmationViewService UserRegistrationConfirmationViewService { get; set; }

        [Inject]
        UserRegistrationConfirmationViewState ViewState { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        public void OnCloseButtonClickHandler()
        {
            UserRegistrationConfirmationViewService.Reset();
        }

        private async Task RestartTimer()
        {
            await UserRegistrationConfirmationViewService.RestartTimerAsync();
        }

        private Task NavigateBackAsync()
        {
            var returnRoute = RegistrationSession.Flow == RegistrationFlow.Email
                ? ClientRoutes.RegistrationEmailRoute
                : ClientRoutes.RegistrationPhoneRoute;

            RegistrationSession.Clear();
            NavigationService.NavigateTo(returnRoute);

            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            Card?.SetBack(this, NavigateBackAsync);
            this.SubscribeChange(ViewState);
            base.OnInitialized();
        }

        public override void Dispose()
        {
            Card?.ClearBack(this);
            this.UnsubscribeChange(ViewState);
            base.Dispose();
        }
    }
}
