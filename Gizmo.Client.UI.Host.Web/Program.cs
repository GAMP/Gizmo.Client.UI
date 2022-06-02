using Gizmo.Client.UI.Host.Web.Services;
using Gizmo.Client.UI.Services;
using Gizmo.Shared.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

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

            #endregion

            hostBuilder.Services.AddSingleton<IComponentDiscoveryService, WebAssemblyComponentDiscoveryService>();

            hostBuilder.Services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "Properties";
            });

            hostBuilder.Services.AddSingleton<IStringLocalizer, StringLocalizer<Resources.Resources>>();
            hostBuilder.Services.AddSingleton<ILocalizationService, UILocalizationService>();

            //add http client factory along with default http client
            hostBuilder.Services.AddHttpClient("Default", cfg => { cfg.BaseAddress = new Uri(hostBuilder.HostEnvironment.BaseAddress); });


            var host = hostBuilder.Build();

            var ds = host.Services.GetRequiredService<IComponentDiscoveryService>();

            await ds.InitializeAsync();
            await host.RunAsync();
        }
    }    
}
