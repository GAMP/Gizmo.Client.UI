using Gizmo.Client.UI.Localization.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Web.Api.Models;

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
            UsageType.TimeOffer => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_TIME_PRODUCTS_TITLE_TIME_OFFER),
            UsageType.TimeFixed => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_TIME_PRODUCTS_TITLE_FIXED_TIME),
            UsageType.Rate => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_TIME_PRODUCTS_TITLE_RATE),
            _ => nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USAGE_TYPE_NONE),
        };

        public static string Name(TimeProductViewState product, GrafitLocalizationService localization) =>
            product.TimeProductType == UsageType.None || string.IsNullOrWhiteSpace(product.TimeProductName)
                ? localization.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USAGE_TYPE_NONE))
                : product.TimeProductName;
    }
}
