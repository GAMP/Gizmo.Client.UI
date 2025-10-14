using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Gizmo.Client.UI.Services;
using Gizmo.UI;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Host.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = WebAssemblyHostBuilder.CreateDefault(args);

            hostBuilder.RootComponents.Add<App>("#app");
            hostBuilder.RootComponents.Add<HeadOutlet>("head::after");

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

            var assemblies = await LoadAssembliesAsync(hostBuilder.Services, hostBuilder.Configuration);

            foreach (var assembly in assemblies)
            {
                Console.WriteLine($"Loaded assembly: {assembly.FullName}");
                hostBuilder.Services.AddClientServices(assembly);
            }

            hostBuilder.Services.AddSingleton<IClientDialogService, ClientDialogService>();
            hostBuilder.Services.AddDialogService<IClientDialogService>();
            
            hostBuilder.Services.AddSingleton<IClientNotificationService, ClientNotificationService>();
            hostBuilder.Services.AddNotificationsService<IClientNotificationService>();

            hostBuilder.Services.AddSingleton<IInputLanguageService, WebInputLenguageService>();

            hostBuilder.Services.AddSingleton<IGizmoClient, DemoClient>();
            hostBuilder.Services.AddSingleton<IImageService, ImageService>();
            hostBuilder.Services.AddSingleton<INotificationsHost, WebNotificationHost>();

            var host = hostBuilder.Build();

            await host.Services.InitializeClientServices();

            await host.RunAsync();
        }
        private static async Task<List<Assembly>> LoadAssembliesAsync(IServiceCollection services, IConfiguration configuration)
        {
            var assemblies = new List<Assembly>();

            using var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            var assemblyNames = configuration.GetSection("UIComposition:AdditionalAssemblies").Get<string[]>();

            foreach (var assemblyName in assemblyNames)
            {
                try
                {
                    var assembly = await LoadAssemblyAsync(httpClientFactory, assemblyName);
                    if (assembly != null && assembly.FullName.Contains(".Module"))
                        assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load assembly {assemblyName}: {ex.Message}");
                }
            }

            return assemblies;
        }

        private static async Task<Assembly> LoadAssemblyAsync(IHttpClientFactory httpClientFactory, string assemblyName)
        {
            using var ct = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            
            using (HttpClient client = httpClientFactory.CreateClient("Default"))
            {
                //the default client should have the base address set so we can start requesting the assembly data with relative path

                //try to fetch external assembly
                var externallib = await client.GetByteArrayAsync($"/_framework/{assemblyName}", ct.Token).ConfigureAwait(false);

                //load the external assembly into app domain
                var assembly = Assembly.Load(externallib);

                //return loaded assembly
                return assembly;
            }
        }

    }
}
