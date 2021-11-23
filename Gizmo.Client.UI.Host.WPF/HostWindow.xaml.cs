using Microsoft.Web.WebView2.Core;
using System.Globalization;
using System.Windows;

namespace Gizmo.Client.UI.Host.WPF
{
    /// <summary>
    /// Interaction logic for HostWindow.xaml
    /// </summary>
    public partial class HostWindow : Window , IHostWindow
    {
        public HostWindow()
        {
            InitializeComponent();
            Loaded += HostWindow_Loaded;
            _blazorView.Loaded += _blazorView_Loaded;
        }

        private void _blazorView_Loaded(object sender, RoutedEventArgs e)
        {
            Language = System.Windows.Markup.XmlLanguage.GetLanguage(new CultureInfo("el-gr").IetfLanguageTag);
        }

        private void HostWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Language = System.Windows.Markup.XmlLanguage.GetLanguage(new CultureInfo("el-gr").IetfLanguageTag);
        }
    }

}
