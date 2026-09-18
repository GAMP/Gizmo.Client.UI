using System;
using System.Globalization;

using Gizmo.Client.UI.Services;
using Gizmo.Web.Components;

namespace Gizmo.Client.UI.Shared
{
    public partial class LoyaltyAvatarRing : CustomDOMComponentBase
    {
        //Circumference of the r=19 circle in the markup.
        private const double CIRCUMFERENCE = 2 * Math.PI * 19;

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

        //Raised on the client's threads; the renderer is reached through the dispatcher.
        private void OnLoyaltyChanged() => DispatchStateHasChanged();

        protected static string RingOffset(double progress) =>
            (CIRCUMFERENCE * (1 - Math.Clamp(progress, 0, 1))).ToString("0.##", CultureInfo.InvariantCulture);
    }
}
