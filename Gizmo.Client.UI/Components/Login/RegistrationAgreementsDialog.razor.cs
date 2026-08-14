using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class RegistrationAgreementsDialog : CustomDOMComponentBase
    {
        private Dictionary<int, bool> _checked = new();

        #region PROPERTIES

        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public IReadOnlyList<RegistrationAgreement> Agreements { get; set; }

        [Parameter]
        public DialogDisplayOptions DisplayOptions { get; set; }

        [Parameter]
        public EventCallback DismissCallback { get; set; }

        [Parameter]
        public EventCallback<RegistrationAgreementsResult> ResultCallback { get; set; }

        #endregion

        #region METHODS

        private bool CanAccept =>
            Agreements.Where(a => !a.IsRejectable).All(a => _checked.GetValueOrDefault(a.Id));

        private async Task AcceptAsync()
        {
            await ResultCallback.InvokeAsync(new RegistrationAgreementsResult
            {
                AllMandatoryAccepted = CanAccept
            });
        }

        private async Task CloseAsync()
        {
            await DismissCallback.InvokeAsync();
        }

        #endregion

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _checked = Agreements.ToDictionary(a => a.Id, _ => false);
        }
    }
}
