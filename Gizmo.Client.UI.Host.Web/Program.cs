using System;
using System.Globalization;
using System.Threading.Tasks;

using Gizmo.Client.UI;
using Gizmo.Client.UI.Services;
using Gizmo.UI;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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

            hostBuilder.Services.AddClientServices();

            hostBuilder.Services.AddSingleton<IClientDialogService, ClientDialogService>();
            hostBuilder.Services.AddDialogService<IClientDialogService>();
            hostBuilder.Services.AddSingleton<IClientNotificationService, ClientNotificationService>();
            hostBuilder.Services.AddNotificationsService<IClientNotificationService>();

            hostBuilder.Services.AddSingleton<IInputLanguageService, WebInputLenguageService>();

            hostBuilder.Services.AddSingleton<IGizmoClient, DemoClient>();
            hostBuilder.Services.AddSingleton<IImageService, ImageService>();
            hostBuilder.Services.AddSingleton<INotificationsHost, WebNotificationHost>();

            var host = hostBuilder.Build();

            // Apply the server default culture BEFORE RunAsync so the Blazor WebAssembly
            // runtime downloads/loads the matching satellite resource assembly for it.
            // Setting the culture only after startup leaves strings on the neutral (English)
            // fallback because the satellite is never loaded.
            try
            {
                var serverInfo = host.Services.GetRequiredService<IServerInfoService>();
                var serverCulture = await serverInfo.GetDefaultCultureAsync();

                if (!string.IsNullOrWhiteSpace(serverCulture))
                {
                    var culture = ResolveCulture(serverCulture);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                    CultureInfo.DefaultThreadCurrentCulture = culture;
                    CultureInfo.DefaultThreadCurrentUICulture = culture;
                }
            }
            catch (Exception ex)
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                logger.LogWarning(ex, "Failed to apply server default culture on startup.");
            }

            await host.Services.InitializeClientServices();

            await host.RunAsync();
        }

        private static CultureInfo ResolveCulture(string cultureName)
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
            return culture.IsNeutralCulture ? CultureInfo.CreateSpecificCulture(culture.Name) : culture;
        }
    }
}
