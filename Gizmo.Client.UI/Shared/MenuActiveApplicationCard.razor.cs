using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class MenuActiveApplicationCard : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        public ActiveApplicationsViewService ActiveApplicationsService { get; set; }

        [Parameter]
        public AppExeViewState Executable { get; set; }

        #region OVERRIDES

        protected override void OnInitialized()
        {
            if (Executable != null)
            {
                this.SubscribeChange(Executable);
            }

            base.OnInitialized();
        }

        public override void Dispose()
        {
            if (Executable != null)
            {
                this.UnsubscribeChange(Executable);
            }

            base.Dispose();
        }

        #endregion
    }
}
