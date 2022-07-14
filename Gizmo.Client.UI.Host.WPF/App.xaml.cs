﻿using Gizmo.Client.UI.Services;
using Gizmo.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

            string skinBasePath = @"D:\My Documents\Visual Studio 2015\Projects\Gizmo\Gizmo.Client.UI\Gizmo.Client.UI.Host.WPF\bin\Release\net6.0-windows\publish";
            string appSettingsFile = Path.Combine(skinBasePath, @"skin.json");


            hostBuilder.ConfigureServices((context, serviceCollection) =>
            {
                serviceCollection.AddLogging();
                serviceCollection.AddBlazorWebViewDeveloperTools();
                serviceCollection.AddWpfBlazorWebView();

                serviceCollection.AddClientOptions(context.Configuration);
                serviceCollection.AddClientServices();
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
