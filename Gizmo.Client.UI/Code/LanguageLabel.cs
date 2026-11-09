using System.Globalization;

namespace Gizmo.Client.UI
{
    public static class LanguageLabel
    {
        public static string Code(CultureInfo culture) => culture.NativeName[..3].ToUpper(culture);

        public static string Name(CultureInfo culture)
        {
            var name = culture.NativeName;
            return string.IsNullOrEmpty(name) ? string.Empty : char.ToUpper(name[0], culture) + name[1..];
        }
    }
}
