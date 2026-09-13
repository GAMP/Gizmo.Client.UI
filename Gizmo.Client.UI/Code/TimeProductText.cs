using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using ShellText = Gizmo.Client.UI.Localization.ShellStringOverrides;

namespace Gizmo.Client.UI
{
    /// <summary>
    /// How a purchased time product is named and pictured, shared by the tariff tooltip
    /// in the top bar and the Time tab of the account page so the two never disagree.
    /// </summary>
    public static class TimeProductText
    {
        /// <summary>
        /// Phosphor icon class for the product's kind.
        /// </summary>
        public static string Icon(UsageType type) => type switch
        {
            UsageType.TimeOffer => "ph-package",
            UsageType.TimeFixed => "ph-hourglass-medium",
            UsageType.Rate => "ph-timer",
            _ => "ph-clock",
        };

        /// <summary>
        /// Resource key of the kind's label ("Time package", "Rate", ...).
        /// </summary>
        public static string TypeKey(UsageType type) => type switch
        {
            UsageType.TimeOffer => ShellText.TIME_TYPE_PACKAGE,
            UsageType.TimeFixed => ShellText.TIME_TYPE_FIXED,
            UsageType.Rate => ShellText.TIME_TYPE_RATE,
            _ => ShellText.TIME_TYPE_NONE,
        };

        /// <summary>
        /// The product's name; plain "Time" for an entry without a kind.
        /// </summary>
        public static string Name(TimeProductViewState product) =>
            product.TimeProductType == UsageType.None || string.IsNullOrWhiteSpace(product.TimeProductName)
                ? ShellText.Get(ShellText.TIME_TYPE_NONE)
                : product.TimeProductName;

        /// <summary>
        /// A stored duration string, or the infinity sign when the product has none.
        /// </summary>
        public static string Span(string value) =>
            string.IsNullOrWhiteSpace(value) ? "∞" : value;
    }
}
