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

        private void OnCardClick() => Service.Highlight(Item.AchievementId);

        private Task OnInfoClick(MouseEventArgs e)
        {
            Service.Highlight(Item.AchievementId);
            return ToggleInfo(e);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && Item.IsHighlighted)
                await InvokeVoidAsync("scrollElementIntoView", Ref);
        }
    }
}
