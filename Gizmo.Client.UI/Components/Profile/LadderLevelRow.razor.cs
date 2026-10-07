using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gizmo.Client.UI.Localization.Resources;
using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Components
{
    public partial class LadderLevelRow : CustomDOMComponentBase
    {
        private const string HistoryPopupSelector = ".giz-ladder-history-wrapper";

        [Inject]
        UserLadderViewService Service { get; set; }

        [Parameter]
        public UserLadderLevelViewState Item { get; set; } = null!;

        [CascadingParameter] protected GrafitLocalizationService GrafitLocalization { get; set; }

        [Parameter]
        public string Rise { get; set; } = "0";

        [Parameter]
        public string Fill { get; set; }

        [Parameter]
        public string Here { get; set; }

        [Parameter]
        public IReadOnlyList<UserLadderRequirementViewState> Requirements { get; set; }

        [Parameter]
        public bool Dense { get; set; }

        private bool IsPassed => Item.IsSatisfied && !Item.IsCurrent && !Item.IsNext;

        private string StepClass
        {
            get
            {
                var css = "giz-stairs__step";

                if (Item.IsCurrent)
                    css += " giz-stairs__step--current";
                else if (Item.IsNext)
                    css += " giz-stairs__step--next";
                else if (IsPassed)
                    css += " giz-stairs__step--passed";

                if (Item.IsLocked)
                    css += " giz-stairs__step--locked";

                return Item.IsSelected ? css + " giz-stairs__step--selected" : css;
            }
        }

        private string StepStyle => Fill is null
            ? $"--giz-step: {Rise}"
            : $"--giz-step: {Rise}; --giz-fill: {Fill}";

        private string MetaText
        {
            get
            {
                if (!string.IsNullOrEmpty(Item.MetaText))
                    return Item.MetaText;

                return Item.HasRequirements
                    ? GrafitLocalization.GetPluralString(GrafitResourceKeys.SHELL_LADDER_CONDITIONS, Item.Requirements.Count)
                    : null;
            }
        }

        private async Task OnClick(MouseEventArgs e)
        {
            await JsRuntime.InvokeVoidAsync("closeOpenPopups", e, HistoryPopupSelector);
            Service.SelectLevel(Item.Rank);
        }
    }
}
