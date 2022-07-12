using Gizmo.Client.UI.Services;
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
        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var servicesAssembly = typeof(View.States.BusinessLogoViewState).Assembly;
            
            var hostBuilder = new HostBuilder();

            string appSettingsFile = @"skin.json";

            hostBuilder
                 .ConfigureServices((context, serviceCollection) =>
                 {
                     serviceCollection.AddLogging();          

                     serviceCollection.AddBlazorWebView();
                     serviceCollection.AddWpfBlazorWebView();
                     serviceCollection.AddClientConfiguration(context.Configuration);

                     serviceCollection.AddSingleton<IHostWindow, HostWindow>();

                     serviceCollection.AddLocalization(opt =>
                     {
                         opt.ResourcesPath = "Properties";
                     });

                     serviceCollection.AddSingleton<IStringLocalizer, StringLocalizer<Resources.Resources>>();

                     serviceCollection.AddClientUIServices();
                     serviceCollection.AddClientViewServices();
                     serviceCollection.AddClientViewStates();                  
                 })

                 .ConfigureLogging(loggingBuilder =>
                 {
                     loggingBuilder.AddConsole();
                     loggingBuilder.SetMinimumLevel(LogLevel.Trace);

                 }).ConfigureAppConfiguration(configurationBuilder =>
                 {
                     configurationBuilder.AddClientConfiguration(appSettingsFile);
                 });


            var host = hostBuilder.Build();

            await host.Services.InitializeClientServices();
            await host.Services.InitializeClientViewServices();

            var serviceProvider = host.Services.GetRequiredService<IServiceProvider>();
            Resources.Add("services", serviceProvider);
            var hostWindow = (HostWindow)serviceProvider.GetRequiredService<IHostWindow>();


            hostWindow.Show();

            await host.StartAsync();

         
        }
    }
}
