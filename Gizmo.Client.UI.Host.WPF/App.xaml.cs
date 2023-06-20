using System;
using System.IO;
using System.Windows;
using Gizmo.Client.UI.Services;
using Gizmo.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            //close application as soon as main window closes
            ShutdownMode = ShutdownMode.OnMainWindowClose;

            var hostBuilder = new HostBuilder();


            hostBuilder.ConfigureServices((context, serviceCollection) =>
            {
                serviceCollection.AddMemoryCache();
                serviceCollection.AddLogging();
                serviceCollection.AddBlazorWebViewDeveloperTools();
                serviceCollection.AddWpfBlazorWebView();

                serviceCollection.AddClientOptions(context.Configuration);
                serviceCollection.AddClientServices();

                serviceCollection.AddDialogService<IClientDialogService>();
                serviceCollection.AddNotificationsService<IClientNotificationService>();

                serviceCollection.AddSingleton<IGizmoClient, TestClient>();
                serviceCollection.AddSingleton<IImageService, ImageService>();

                serviceCollection.AddSingleton<IInputLanguageService, WpfInputLenguageService>();

                serviceCollection.AddSingleton<IHostWindow, HostWindow>();
                serviceCollection.AddSingleton<INotificationsHost,NotificationsHost>();

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
         
            var compositionService = host.Services.GetRequiredService<DesktopUICompositionService>();

            string compositionFile = Path.Combine(Environment.CurrentDirectory, @"composition.json");
            string optionsFile = Path.Combine(Environment.CurrentDirectory, @"options.json");

            await compositionService.SetConfigurationSourceAsync(compositionFile);
            await compositionService.SetOptionsConfigurationSourceAsync(optionsFile);
            await compositionService.InitializeAsync(default);

            //initialize services
            await host.Services.InitializeClientServices();

            //create host window
            var hostWindow = (HostWindow)host.Services.GetRequiredService<IHostWindow>();

            //create notifications window (early load)
            var notificationsHost = host.Services.GetRequiredService<INotificationsHost>();

            //show host window
            hostWindow.Show();

            await host.StartAsync();
        }
    }
}
