using Gizmo.Client.UI.Services;
using Gizmo.Shared.Services;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;

namespace Gizmo.Client.UI.Host.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var greekCulture = new CultureInfo("el-gr");

            var hostBuilder = new HostBuilder();

            string appSettingsFile = @"D:\My Documents\Visual Studio 2015\Projects\Gizmo\Gizmo.Client.UI\Gizmo.Client.UI\bin\Release\net6.0\skin.json";

            hostBuilder
                 .ConfigureServices((context, serviceCollection) =>
                 {
                     serviceCollection.AddBlazorWebView();
                     serviceCollection.AddClientConfiguration(context.Configuration);

                     serviceCollection.AddSingleton<IHostWindow, HostWindow>();

                     serviceCollection.AddLocalization(opt =>
                     {
                         opt.ResourcesPath = "Properties";
                     });

                     serviceCollection.AddSingleton<IStringLocalizer, StringLocalizer<Resources.Resources>>();
                     serviceCollection.AddSingleton<ILocalizationService, UILocalizationService>();
                     serviceCollection.AddSingleton<IComponentDiscoveryService, DesktopComponentDiscoveryService>();
                 })

                 .ConfigureLogging(loggingBuilder =>
                 {
                     loggingBuilder.AddConsole();

                 }).ConfigureAppConfiguration(configurationBuilder =>
                 {
                     configurationBuilder.AddClientConfiguration(appSettingsFile);
                 });

            var host = hostBuilder.Build();         

            var localizer = host.Services.GetRequiredService<IStringLocalizer<Resources.Resources>>();
            var vd = localizer.GetAllStrings();

            var ls = host.Services.GetRequiredService<ILocalizationService>();           
            
            ls.SetCurrentCulture(greekCulture);
       

            var serviceProvider = host.Services.GetRequiredService<IServiceProvider>();
            Resources.Add("services", serviceProvider);

            var ds = host.Services.GetRequiredService<IComponentDiscoveryService>();

            await ds.InitializeAsync(default);

            var hostWindow = (HostWindow)serviceProvider.GetRequiredService<IHostWindow>();

            hostWindow._blazorView.HostPage = @"D:\My Documents\Visual Studio 2015\Projects\Gizmo\Gizmo.Client.UI\Gizmo.Client.UI.Host.WPF\bin\Debug\net6.0-windows\wwwroot\index.html";
            var rootComponent = new RootComponent()
            {
                ComponentType = ds.RootComponentType,
                Selector = "#app",
            };
            hostWindow._blazorView.RootComponents.Add(rootComponent);
            hostWindow.Show();

        }
    }
}
