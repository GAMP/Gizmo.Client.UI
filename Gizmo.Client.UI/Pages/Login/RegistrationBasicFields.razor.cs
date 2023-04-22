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
        UserRegistrationConfirmationMethodViewService UserRegistrationConfirmationMethodService { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewService UserRegistrationBasicFieldsService { get; set; }

        [Inject]
        UserRegistrationBasicFieldsViewState ViewState { get; set; }

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
