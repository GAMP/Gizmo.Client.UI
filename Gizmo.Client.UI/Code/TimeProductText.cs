using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;
using ShellText = Gizmo.Client.UI.Localization.ShellStringOverrides;

namespace Gizmo.Client.UI
{
    public static class TimeProductText
    {
        public static string Icon(UsageType type) => type switch
        {
            UsageType.TimeOffer => "ph-package",
            UsageType.TimeFixed => "ph-hourglass-medium",
            UsageType.Rate => "ph-timer",
            _ => "ph-clock",
        };

        public static string TypeKey(UsageType type) => type switch
        {
            UsageType.TimeOffer => ShellText.TIME_TYPE_PACKAGE,
            UsageType.TimeFixed => ShellText.TIME_TYPE_FIXED,
            UsageType.Rate => ShellText.TIME_TYPE_RATE,
            _ => ShellText.TIME_TYPE_NONE,
        };

        public static string Name(TimeProductViewState product) =>
            product.TimeProductType == UsageType.None || string.IsNullOrWhiteSpace(product.TimeProductName)
                ? ShellText.Get(ShellText.TIME_TYPE_NONE)
                : product.TimeProductName;

        public static string Span(string value) =>
            string.IsNullOrWhiteSpace(value) ? "∞" : value;
    }
}
