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
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationConfirmationViewService UserRegistrationConfirmationViewService { get; set; }

        [Inject]
        UserRegistrationConfirmationViewState ViewState { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

        private string GetPlaceholder()
        {
            var len = RegistrationSession.CodeLength;
            if (len <= 3)
                return "123".Substring(0, len);
            else if (len == 4)
                return "12 34".Substring(0, len + 1);
            else if (len >= 5 && len <= 6)
                return "123 456".Substring(0, len + 1);
            else
                return "1234 5678".Substring(0, len + 1);
        }

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
