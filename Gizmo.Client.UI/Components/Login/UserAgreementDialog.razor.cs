using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Components
{
    public partial class UserAgreementDialog : CustomDOMComponentBase
    {
        private bool _accepted;

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Agreement { get; set; }

        [Parameter]
        public bool IsRejectable { get; set; }

        /// <summary>
        /// Allows the user to continue without checking the accept checkbox, even for non-rejectable agreements.
        /// Used by the registration flow, where declining a mandatory agreement routes the user back to login.
        /// </summary>
        [Parameter]
        public bool AllowContinueWithoutAccept { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<UserAgreementResult> ResultCallback { get; set; }

        #endregion

        #region METHODS

        private async Task CloseDialogAsync()
        {
            await DismissCallback.InvokeAsync();
        }

        private async Task ContinueAsync()
        {
            await ResultCallback.InvokeAsync(new UserAgreementResult() { Accepted = _accepted});
        }

        #endregion

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _accepted = false;
        }
    }
}
