using Gizmo.Client.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var hostBuilder = new HostBuilder();

            hostBuilder
                 .ConfigureServices((context, serviceCollection) =>
                 {
                     serviceCollection.AddBlazorWebView();
                     serviceCollection.AddSingleton<IComponentDiscoveryService, DesktopComponentDiscoveryService>();
                     serviceCollection.AddClientConfiguration(context.Configuration);

                 }).ConfigureLogging(loggingBuilder =>
                 {
                     loggingBuilder.AddConsole();

                 }).ConfigureAppConfiguration(configurationBuilder =>
                 {
                     configurationBuilder.AddClientConfiguration();
                 });


            var host = hostBuilder.Build();

            var serviceProvider = host.Services.GetRequiredService<IServiceProvider>();
            Resources.Add("services", serviceProvider);

            var ds = host.Services.GetRequiredService<IComponentDiscoveryService>();

            await ds.InitializeAsync(default);

            await host.RunAsync();
           
        }
    }
}
