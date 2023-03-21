using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class UserAgreementDialog : CustomDOMComponentBase
    {
        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserAgreementsService UserAgreementsService { get; set; }

        [Inject]
        UserAgreementsViewState ViewState { get; set; }

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        [Parameter]
        public EventCallback<UserAgreementResult> ResultCallback { get; set; }

        #endregion

        #region METHODS

        private void ChangeAcceptState(bool value)
        {
            if (value)
            {
                UserAgreementsService.SetCurrentUserAgreementState(UserAgreementAcceptState.Accepted);
            }
            else
            {
                UserAgreementsService.SetCurrentUserAgreementState(UserAgreementAcceptState.Rejected);
            }
        }

        private async Task CloseDialogAsync()
        {
            await CancelCallback.InvokeAsync();
        }

        private async Task GetNextUserAgreementAsync()
        {
            await ResultCallback.InvokeAsync(new UserAgreementResult());
        }

        #endregion

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
