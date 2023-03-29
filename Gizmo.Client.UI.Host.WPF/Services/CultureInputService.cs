using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Gizmo.UI;

namespace Gizmo.Client.UI.Host.WPF
{
    public sealed class CultureInputService : ICultureService
    {
        public CultureInputService() 
        {
            InputLanguageManager.Current.InputLanguageChanged += Current_InputLanguageChanged;
        }

        private void Current_InputLanguageChanged(object sender, InputLanguageEventArgs e)
        {
        }

        public IEnumerable<CultureInfo> AveliableCultures => InputLanguageManager.Current.AvailableInputLanguages.OfType<CultureInfo>();


        public async Task SetCurrentCultureAsync(CultureInfo culture)
        {
            await Application.Current?.Dispatcher.InvokeAsync(() => InputLanguageManager.Current.CurrentInputLanguage = culture);
        }

        public CultureInfo GetCurrentCulture(string twoLetterISOLanguageName) =>
            AveliableCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentCulture;
    }
}
