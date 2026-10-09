using System;
using System.Globalization;

namespace Gizmo.Client.UI
{
    public static class CssValue
    {
        public static string Number(decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);

        public static string Percent(decimal value) => Number(decimal.Clamp(value, 0, 100)) + "%";

        public static string Percent(double value) => Math.Clamp(value, 0, 100).ToString("0.##", CultureInfo.InvariantCulture) + "%";
    }
}
