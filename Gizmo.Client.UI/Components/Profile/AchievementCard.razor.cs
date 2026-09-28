using System.Globalization;
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

        private string CardClass
        {
            get
            {
                var css = "giz-achievement-card";

                if (Item.IsEarned)
                    css += " giz-achievement-card--earned";

                if (IsInfoOpen)
                    css += " giz-achievement-card--active";

                return Item.IsHighlighted ? css + " giz-achievement-card--highlighted" : css;
            }
        }

        private string ChipClass => Item.IsEarned
            ? "giz-achievement-card__chip giz-achievement-card__chip--success"
            : "giz-achievement-card__chip";

        private bool ShowMeter => !Item.IsEarned && Item.ShowProgressBar;

        private string ProgressValue => Item.ProgressPercent.ToString("0.##", CultureInfo.InvariantCulture);

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
