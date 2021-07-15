using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Gizmo.Client.UI.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBlazorWebView();
            var provider = serviceCollection.BuildServiceProvider();
            Resources.Add("services", provider);

            base.OnStartup(e);
        }

    }
}
