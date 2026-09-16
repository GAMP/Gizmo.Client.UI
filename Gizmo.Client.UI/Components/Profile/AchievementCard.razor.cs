using System.Threading.Tasks;
using Gizmo.Client.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Gizmo.Client.UI.Components
{
    public partial class AchievementCard : ProfileCardBase
    {
        [Inject]
        UserAchievementsViewService Service { get; set; }

        [Parameter] public UserAchievementViewState Item { get; set; } = null!;

        // a click anywhere on the card moves the sticky highlight here (the popup stops propagation)
        private void OnCardClick() => Service.Highlight(Item.AchievementId);

        private Task OnInfoClick(MouseEventArgs e)
        {
            Service.Highlight(Item.AchievementId);
            return ToggleInfo(e);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            // deep-link arrival: the highlighted card is rendered once after load — bring it into view
            if (firstRender && Item.IsHighlighted)
                await InvokeVoidAsync("scrollElementIntoView", Ref);
        }
    }
}
