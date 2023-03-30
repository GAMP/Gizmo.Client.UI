using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Gizmo.UI;

namespace Gizmo.Client.UI.Host.WPF
{
    public sealed class WpfInputLenguageService : IInputLanguageService
    {
        public WpfInputLenguageService()
        {
            InputLanguageManager.Current.InputLanguageChanged += Current_InputLanguageChanged;
        }

        private void Current_InputLanguageChanged(object sender, InputLanguageEventArgs e)
        {
        }

        public IEnumerable<CultureInfo> AvailableInputLanguages => InputLanguageManager.Current.AvailableInputLanguages.OfType<CultureInfo>();


        public async Task SetCurrentInputLanguageAsync(CultureInfo culture)
        {
            await Application.Current?.Dispatcher.InvokeAsync(() => InputLanguageManager.Current.CurrentInputLanguage = culture);
        }

        public CultureInfo GetLanguage(string twoLetterISOLanguageName)
        {
            return AvailableInputLanguages.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName);
        }
    }
}
