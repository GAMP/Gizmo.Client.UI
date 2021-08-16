using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace Gizmo.Client.UI.Services
{
    public sealed class WebAssemblyComponentDiscoveryService : ComponentDiscoveryServiceBase
    {
        #region CONSTRUCTOR
        public WebAssemblyComponentDiscoveryService(IConfiguration configuration,
              IServiceProvider serviceProvider,
              IHttpClientFactory httpClientFactory,
              ILogger<WebAssemblyComponentDiscoveryService> logger) : base(configuration, logger, serviceProvider)
        {
            _appAssembly = typeof(App).Assembly;
            _httpClientFactory = httpClientFactory;
        }
        #endregion

        #region FIELDS
        private readonly IHttpClientFactory _httpClientFactory;
        #endregion

        #region PROPERTIES
        #endregion

        #region OVERRIDES

        protected async override Task LoadAssemblyAsync(string assemblyName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName));

            using (HttpClient client = _httpClientFactory.CreateClient("Default"))
            {
                //the default client should have the base address set so we can start requesting the assembly data with relative path

                //try to fetch external assembly
                var externallib = await client.GetByteArrayAsync($"/_framework/{assemblyName}", ct).ConfigureAwait(false);

                //load the external assembly into app domain
                var assembly = AppDomain.CurrentDomain.Load(externallib);

                //on sucess add the assembly to additional assemblies list
                _addtionalAssemblies.Add(assembly);
            }
        }

        protected override IEnumerable<string> GetRoutes(Type type)
        {
            return type.GetCustomAttributes<RouteAttribute>().Select(attribute => attribute.Template).ToArray();
        }

        #endregion
    }
}