using System;
using System.Globalization;

using Gizmo.Client.UI.Services;
using Gizmo.Web.Api.Models;
using Gizmo.Web.Components;

namespace Gizmo.Client.UI.Pages
{
    public partial class Progress : ShellComponentBase
    {
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

        /// <summary>Position on the rail, in percent of the top level's threshold.</summary>
        protected static string Pct(decimal value, decimal max) =>
            (Math.Clamp((double)(value / max), 0, 1) * 100).ToString("0.#", CultureInfo.InvariantCulture);

        //Running challenges first, the nearest to done on top; finished ones after; the
        //rest (ended, blocked, ineligible) last.
        protected static int ChallengeOrder(UserAchievementChallengeModel c)
        {
            if (LoyaltySnapshot.IsDone(c))
                return 1;
            if (LoyaltySnapshot.IsOpen(c))
                return 0;
            return 2;
        }

        //Earned first, then by how close the rest are; secrets at the end.
        protected static double AchievementOrder(UserAchievementModel a)
        {
            if (LoyaltySnapshot.IsSecret(a))
                return 2;
            if (LoyaltySnapshot.IsEarned(a))
                return 0;
            return 1 + (1 - LoyaltySnapshot.AchievementFraction(a));
        }
    }
}
