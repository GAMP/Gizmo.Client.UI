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
    public sealed class CultureOutputService : ICultureOutputService
    {
        private readonly ILocalizationService _localizationService;
        public CultureOutputService(ILocalizationService localizationService) => _localizationService = localizationService;
        
        public IEnumerable<CultureInfo> AvailableCultures => 
            _localizationService.SupportedCultures
                .DistinctBy(x => x.TwoLetterISOLanguageName)
                .Select(x => new CultureInfo(x.Name));

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
