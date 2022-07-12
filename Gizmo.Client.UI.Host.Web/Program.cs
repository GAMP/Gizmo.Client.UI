using Gizmo.Client.UI.Services;
using Gizmo.Shared.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Host.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = WebAssemblyHostBuilder.CreateDefault(args);

            hostBuilder.RootComponents.Add<App>("#app");

            #region CONFIGURATION

            hostBuilder.Configuration.AddClientConfiguration();
            hostBuilder.Services.AddClientConfiguration(hostBuilder.Configuration);
  
            #endregion

            #region LOGGING

            //add logging service
            hostBuilder.Services.AddLogging();
            hostBuilder.Logging.SetMinimumLevel(LogLevel.Trace);
            #endregion

            hostBuilder.Services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "Properties";
            });

            hostBuilder.Services.AddSingleton<IStringLocalizer, StringLocalizer<Resources.Resources>>();
            hostBuilder.Services.AddSingleton<ILocalizationService, UILocalizationService>();

            //add http client factory along with default http client
            hostBuilder.Services.AddHttpClient("Default", cfg => { cfg.BaseAddress = new Uri(hostBuilder.HostEnvironment.BaseAddress); });

            hostBuilder.Services.AddClientUIServices();
            hostBuilder.Services.AddClientViewServices();
            hostBuilder.Services.AddClientViewStates();          

            var host = hostBuilder.Build();

            await host.Services.InitializeClientServices();
            await host.Services.InitializeClientViewServices();       

            await host.RunAsync();
        }
    }    
}
