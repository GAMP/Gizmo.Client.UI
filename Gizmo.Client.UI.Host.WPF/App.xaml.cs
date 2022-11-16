using Gizmo.Client.UI.Services;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Windows;

namespace Gizmo.Client.UI.Host.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var hostBuilder = new HostBuilder();
            string appSettingsFile = Path.Combine(Environment.CurrentDirectory, @"skin.json");


            hostBuilder.ConfigureServices((context, serviceCollection) =>
            {
                serviceCollection.AddMemoryCache();
                serviceCollection.AddLogging();
                serviceCollection.AddBlazorWebViewDeveloperTools();
                serviceCollection.AddWpfBlazorWebView();

                serviceCollection.AddClientOptions(context.Configuration);
                serviceCollection.AddClientServices();
                serviceCollection.AddSingleton<IGizmoClient, TestClient>();
                serviceCollection.AddSingleton<IHostWindow, HostWindow>();
              
            }).ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);

            }).ConfigureAppConfiguration(configurationBuilder =>
            {
                //add skin configuration source
                configurationBuilder.AddClientConfigurationSource();
            });

            var host = hostBuilder.Build();

            Resources.Add("services", host.Services);

            //get host window
            var hostWindow = (HostWindow)host.Services.GetRequiredService<IHostWindow>();
            var ds = host.Services.GetRequiredService<DesktopComponentDiscoveryService>();

            //initialize services
            await host.Services.InitializeClientServices();

            await ds.SetConfigurationSourceAsync(appSettingsFile);

            //show host window
            hostWindow.Show();

            await host.StartAsync();
        }
    }
}
