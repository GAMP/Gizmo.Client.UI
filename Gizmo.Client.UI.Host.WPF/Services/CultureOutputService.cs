using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Gizmo.UI;

namespace Gizmo.Client.UI.Host.WPF
{
    public sealed class CultureOutputService : ICultureService
    {
        public IEnumerable<CultureInfo> AveliableCultures => InputLanguageManager.Current.AvailableInputLanguages.OfType<CultureInfo>();

        /// <summary>
        /// Sets current UI culture.
        /// </summary>
        /// <param name="culture">Culture.</param>
        public async Task SetCurrentCultureAsync(CultureInfo culture)
        {
            await Application.Current?.Dispatcher.InvokeAsync(new Action(() => 
            {
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentUICulture = culture; 
            }));
        }

        public CultureInfo GetCurrentCulture(string twoLetterISOLanguageName) =>
            AveliableCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentCulture;
    }
}
