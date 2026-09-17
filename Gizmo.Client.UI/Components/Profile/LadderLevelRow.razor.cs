using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.Web.Components;
using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class LadderLevelRow : CustomDOMComponentBase
    {
        [Inject]
        ILocalizationService LocalizationService { get; set; }

        [Inject]
        UserLadderViewService Service { get; set; }

        [Parameter]
        public UserLadderLevelViewState Item { get; set; } = null!;

        private void OnClick() => Service.SelectLevel(Item.Rank);
    }
}
