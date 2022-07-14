using Gizmo.Client.UI.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
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

            hostBuilder.Configuration.AddClientConfigurationSource();
            hostBuilder.Services.AddClientOptions(hostBuilder.Configuration);
  
            #endregion

            #region LOGGING

            //add logging service
            hostBuilder.Services.AddLogging();
            hostBuilder.Logging.SetMinimumLevel(LogLevel.Trace);
            #endregion

            //add http client factory along with default http client
            hostBuilder.Services.AddHttpClient("Default", cfg => { cfg.BaseAddress = new Uri(hostBuilder.HostEnvironment.BaseAddress); });

            hostBuilder.Services.AddClientServices();       

            var host = hostBuilder.Build();

            await host.Services.InitializeClientServices();     

            await host.RunAsync();
        }
    }    
}
