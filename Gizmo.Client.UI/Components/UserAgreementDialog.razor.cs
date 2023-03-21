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

        [Parameter]
        public EventCallback CancelCallback { get; set; }

        [Parameter]
        public EventCallback<UserAgreementResult> ResultCallback { get; set; }

        #endregion

        #region METHODS

        private async Task CloseDialogAsync()
        {
            await CancelCallback.InvokeAsync();
        }

        private async Task GetNextUserAgreementAsync()
        {
            await ResultCallback.InvokeAsync(new UserAgreementResult());
        }

        #endregion

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _accepted = false;
        }
    }
}
