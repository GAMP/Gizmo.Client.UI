using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gizmo.Client.UI.Host.WPF
{
    public sealed class InputLanguagesService : IInputLanguageService
    {
        public InputLanguagesService() 
        {
            InputLanguageManager.Current.InputLanguageChanged += Current_InputLanguageChanged;
        }

        private void Current_InputLanguageChanged(object sender, InputLanguageEventArgs e)
        {
        }

        public IEnumerable<CultureInfo> AvailableInputLanguages => InputLanguageManager.Current.AvailableInputLanguages.OfType<CultureInfo>();

        public CultureInfo CurrentInputLanguage
        { 
            get => InputLanguageManager.Current.CurrentInputLanguage;
        }

        public async Task SetCurrentLanguageAsync(CultureInfo inputLanguage)
        {
           await Application.Current?.Dispatcher.InvokeAsync(() => InputLanguageManager.Current.CurrentInputLanguage = inputLanguage);          
        }
    }
}
