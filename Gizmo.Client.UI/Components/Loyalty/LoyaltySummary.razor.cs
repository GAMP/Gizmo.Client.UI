using System;
using System.Globalization;
using System.Linq;

using Gizmo.Client.UI.Services;
using Gizmo.UI.Services;
using Gizmo.Web.Components;

using Microsoft.AspNetCore.Components;

namespace Gizmo.Client.UI.Components
{
    public partial class LoyaltySummary : ShellComponentBase
    {
        [Inject] NavigationService NavigationService { get; set; }

        protected static string ProgressRoute => ClientRoutes.UserProfileRoute + "/progress";

        protected override void OnInitialized()
        {
            Loyalty.Changed += OnLoyaltyChanged;

            base.OnInitialized();
        }

        public override void Dispose()
        {
            Loyalty.Changed -= OnLoyaltyChanged;

            base.Dispose();
        }

        private void OnLoyaltyChanged() => DispatchRender();

        private void OpenProgress() => NavigationService.NavigateTo(ProgressRoute);
    }

    /// <summary>
    /// Figures the ring and bar markup needs, in the invariant culture the attributes
    /// require, plus the reward chip of a next step.
    /// </summary>
    public static class LoyaltyRing
    {
        //Circumference of the r=18 circle every ring in the loyalty markup is drawn with.
        private const double CIRCUMFERENCE = 2 * Math.PI * 18;

        public static string Offset(double progress) =>
            (CIRCUMFERENCE * (1 - Math.Clamp(progress, 0, 1))).ToString("0.##", CultureInfo.InvariantCulture);

        public static string Percent(double progress) =>
            (Math.Clamp(progress, 0, 1) * 100).ToString("0.#", CultureInfo.InvariantCulture);

        /// <summary>The first reward of the challenge behind a next step, as a chip; null without one.</summary>
        public static string RewardChip(LoyaltyAction action)
        {
            var challenge = action?.Challenge;
            if (challenge is null)
                return null;

            var points = challenge.PointsRewards?.FirstOrDefault();
            if (points is not null)
                return LoyaltyText.RewardPoints(points.Amount);

            var time = challenge.TimeRewards?.FirstOrDefault();
            if (time is not null)
                return LoyaltyText.RewardTime(time.Seconds);

            var product = challenge.ProductRewards?.FirstOrDefault();
            if (product is not null)
                return LoyaltyText.RewardProduct(product.Quantity);

            return null;
        }
    }
}
