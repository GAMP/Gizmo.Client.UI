using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Gizmo.UI;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.Host.WPF
{
    public sealed class WpfLocalizationService : LocalizationServiceBase
    {
        public WpfLocalizationService(
            ILogger<WebLocalizationService> logger,
            IStringLocalizer localizer,
            IOptions<ClientUIOptions> options) : base(logger, localizer, options)
        {
            var prop = localizer.GetType().GetField("_localizer", BindingFlags.NonPublic | BindingFlags.Instance);
            _resourceManagerStringLocalizer = (ResourceManagerStringLocalizer)prop.GetValue(localizer);
            prop = _resourceManagerStringLocalizer.GetType().GetField("_resourceManager", BindingFlags.NonPublic | BindingFlags.Instance);
            _resourceManager = (ResourceManager)prop.GetValue(_resourceManagerStringLocalizer);
        }

        private readonly ILocalizationService _localizationService;
        public WpfLocalizationService(ILocalizationService localizationService) => _localizationService = localizationService;

        public override IEnumerable<CultureInfo> SupportedCultures
        {
            get
            {
                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

                var supportedCultures = cultures.Where(culture =>
                {
                    try
                    {
                        ResourceSet? resourceSet = _resourceManager?.GetResourceSet(culture, true, false);
                        return resourceSet != null;
                    }
                    catch (CultureNotFoundException ex)
                    {
                        Logger.LogError(ex, "Could not obtain resource set for {culture}.", culture);
                        return false;
                    }
                }).ToList();

                //replace invariant culture with default english
                if (supportedCultures.Contains(CultureInfo.InvariantCulture))
                {
                    supportedCultures.Remove(CultureInfo.InvariantCulture);
                    supportedCultures.Insert(0, CultureInfo.GetCultureInfo("en-us"));
                }

                OverrideCultureCurrencyConfiguration(supportedCultures);

                return supportedCultures;
            }
        }

        /// <summary>
        /// Sets current UI culture.
        /// </summary>
        /// <param name="culture">Culture.</param>
        public async Task SetCurrentCultureAsync(CultureInfo culture)
        {
            await Application.Current?.Dispatcher.InvokeAsync(new Action(() =>
            {
                CultureInfo.CurrentUICulture = culture;
            }));
        }

        public CultureInfo GetCulture(IEnumerable<CultureInfo> cultures, string twoLetterISOLanguageName) =>
            cultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentCulture;
    }
}
