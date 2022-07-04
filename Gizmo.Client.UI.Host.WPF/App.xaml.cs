using Gizmo.Client.UI.Services;
using Gizmo.Shared.Services;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace Gizmo.Client.UI.Host.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var hostBuilder = new HostBuilder();

            string appSettingsFile = @"skin.json";

            hostBuilder
                 .ConfigureServices((context, serviceCollection) =>
                 {
                     serviceCollection.AddViewStates();
                     serviceCollection.AddViewServices();

                     serviceCollection.AddBlazorWebView();
                     serviceCollection.AddWpfBlazorWebView();
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

            host.Services.InitializeViewsServices().GetAwaiter().GetResult();

            var serviceProvider = host.Services.GetRequiredService<IServiceProvider>();
            Resources.Add("services", serviceProvider);

            var ds = host.Services.GetRequiredService<IComponentDiscoveryService>();

            ds.InitializeAsync(default).GetAwaiter().GetResult();

            var hostWindow = (HostWindow)serviceProvider.GetRequiredService<IHostWindow>();

            //set host page
            hostWindow._blazorView.HostPage = @"wwwroot\index.html";

            //create root component
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
