using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Localization.Services
{
    // Grafit's resources first, the vendor's localization service for everything else. A
    // language file answers first; a key found only in the neutral file is Grafit's own and
    // answers in English rather than as a raw key; any other key goes to the vendor.
    public sealed class GrafitLocalizationService : ILocalizationService
    {
        private const string RESOURCES = "Gizmo.Client.UI.Localization.Resources.GrafitResources";

        // Every language is embedded in Gizmo.Client.UI.dll: a skin ships no satellite assemblies.
        private static readonly ConcurrentDictionary<string, ResourceSet> _resourceSets = new();

        private readonly ILocalizationService _vendorLocalizationService;

        public GrafitLocalizationService(ILocalizationService vendorLocalizationService)
        {
            _vendorLocalizationService = vendorLocalizationService ?? throw new ArgumentNullException(nameof(vendorLocalizationService));
        }

        public event EventHandler<EventArgs> LocalizationOptionsChanged
        {
            add => _vendorLocalizationService.LocalizationOptionsChanged += value;
            remove => _vendorLocalizationService.LocalizationOptionsChanged -= value;
        }

        public event EventHandler<EventArgs> LanguageChanged
        {
            add => _vendorLocalizationService.LanguageChanged += value;
            remove => _vendorLocalizationService.LanguageChanged -= value;
        }

        public Task SetCurrentCultureAsync(CultureInfo culture) =>
            _vendorLocalizationService.SetCurrentCultureAsync(culture);

        public ValueTask<IEnumerable<CultureInfo>> GetSupportedCulturesAsync(CancellationToken cToken = default) =>
            _vendorLocalizationService.GetSupportedCulturesAsync(cToken);

        public string GetString(string key) =>
            Find(key) ?? _vendorLocalizationService.GetString(key);

        public string GetString(string key, params object[] arguments)
        {
            var format = Find(key);

            return format is null
                ? _vendorLocalizationService.GetString(key, arguments)
                : Format(format, arguments);
        }

        public string GetStringUpper(string key) => GetString(key).ToUpper();

        public string GetStringUpper(string key, params object[] arguments) => GetString(key, arguments).ToUpper();

        public string GetStringLower(string key) => GetString(key).ToLower();

        public string GetStringLower(string key, params object[] arguments) => GetString(key, arguments).ToLower();

        // The forms are separated by '|' in the order PluralIndex returns; the count is {0}.
        public string GetPluralString(string key, int count)
        {
            var forms = GetString(key).Split('|');
            var index = PluralIndex(count, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            return Format(forms[Math.Min(index, forms.Length - 1)], [count.ToString("N0", CultureInfo.CurrentCulture)]);
        }

        private static string Find(string key)
        {
            return ResourceSetFor(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)?.GetString(key)
                ?? ResourceSetFor(null)?.GetString(key);
        }

        private static ResourceSet ResourceSetFor(string language)
        {
            var name = language is null ? RESOURCES : $"{RESOURCES}.{language}";

            return _resourceSets.GetOrAdd(name, static resourceName =>
            {
                var stream = typeof(GrafitLocalizationService).Assembly.GetManifestResourceStream($"{resourceName}.resources");

                return stream is null ? null : new ResourceSet(stream);
            });
        }

        // Russian and its neighbours take three forms, Slovenian four (one, two, few, other); two
        // cover English and the other languages the client ships. A language with fewer forms
        // than the index uses its last.
        private static int PluralIndex(int count, string language)
        {
            if (language is "sl")
            {
                return (count % 100) switch
                {
                    1 => 0,
                    2 => 1,
                    3 or 4 => 2,
                    _ => 3,
                };
            }

            if (language is "ru" or "uk" or "be")
            {
                var mod100 = count % 100;
                if (mod100 is >= 11 and <= 14)
                    return 2;

                return (count % 10) switch
                {
                    1 => 0,
                    2 or 3 or 4 => 1,
                    _ => 2,
                };
            }

            return count == 1 ? 0 : 1;
        }

        private static string Format(string format, object[] arguments)
        {
            if (arguments is null || arguments.Length == 0 || string.IsNullOrEmpty(format))
                return format;

            try
            {
                return string.Format(CultureInfo.CurrentCulture, format, arguments);
            }
            catch (FormatException)
            {
                return format;
            }
        }
    }
}
