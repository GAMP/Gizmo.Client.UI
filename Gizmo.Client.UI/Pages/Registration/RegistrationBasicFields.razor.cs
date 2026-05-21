using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Pages
{
    [Route(ClientRoutes.RegistrationBasicFieldsRoute)]
    public partial class RegistrationBasicFields : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserRegistrationViewState UserRegistrationViewState { get; set; }

        [Inject]
        IRegistrationSessionService RegistrationSession { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewService UserRegistrationBasicFieldsViewService { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewState ViewState { get; set; }

        [Inject]
        NavigationService NavigationService { get; set; }

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
                _                      => ClientRoutes.RegistrationProvidersRoute // redirect flow (Flow=None, Token non-empty)
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
