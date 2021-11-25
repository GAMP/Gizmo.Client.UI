using Microsoft.Web.WebView2.Core;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
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

            _options.Language = "el-gr";

            //create the web browser environment
            //program files are blocked and web browser wont be able to create user data folder, so it seems this is unavoidable
            _environment = CoreWebView2Environment.CreateAsync(null, @"c:\edge", _options).GetAwaiter().GetResult();

            Loaded += HostWindow_Loaded;
            _blazorView.Loaded += _blazorView_Loaded;
        }

        private readonly CoreWebView2EnvironmentOptions _options = new CoreWebView2EnvironmentOptions();
        private readonly CoreWebView2Environment _environment;


        private async Task InitializeAsync()
        {
            var webView = _blazorView.WebView;

            //ensure web view with desired environment
            await webView.EnsureCoreWebView2Async(_environment);

            webView.CoreWebView2.CookieManager.DeleteAllCookies();

           // webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
           // webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            //webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        }

        private async void _blazorView_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeAsync();

            Language = System.Windows.Markup.XmlLanguage.GetLanguage(new CultureInfo("el-gr").IetfLanguageTag);
        }

        private void HostWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Language = System.Windows.Markup.XmlLanguage.GetLanguage(new CultureInfo("el-gr").IetfLanguageTag);
        }
    }

}
