using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class PasswordTooltip : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Parameter]
        public PasswordTooltipViewState PasswordTooltipViewState { get; set; }

        private string GetColor()
        {
            if (PasswordTooltipViewState.PassedRules == 0)
            {
                return "#EB5757";
            }
            else if (PasswordTooltipViewState.PassedRules == PasswordTooltipViewState.TotalRules)
            {
                return "#219653"; //TODO: A ICON
            }
            else
            {
                return "#F2994A";
            }
            /*case 2:
                return "#F2994A";

            case 3:
                return "#F2C94C";*/

        }

        protected override void OnInitialized()
        {
            this.SubscribeChange(PasswordTooltipViewState);

            base.OnInitialized();
        }

        public override void Dispose()
        {
            this.UnsubscribeChange(PasswordTooltipViewState);

            base.Dispose();
        }
    }
}
