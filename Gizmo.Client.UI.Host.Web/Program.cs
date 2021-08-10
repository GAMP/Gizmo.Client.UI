using Gizmo.Shared.ViewModels;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Host.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ////create services variable
            //var services = builder.Services;
            //#region VIEW MODELS

            ////add by type
            //foreach (var type in Assembly.GetExecutingAssembly().ExportedTypes)
            //{
            //    //get type info
            //    var typeInfo = type.GetTypeInfo();

            //    //must be a class
            //    if (!typeInfo.IsClass)
            //        continue;

            //    //must be non abstract
            //    if (typeInfo.IsAbstract)
            //        continue;

            //    //attributes can be checked here for client exclusion e.t.c

            //    //any type that implement IPageViewModel added as singelton service
            //    if (typeInfo.GetInterfaces().Any(t => t == typeof(IPageViewModel)))
            //        services.AddSingleton(type);

            //    //any type that implement IComponentViewModel added as singelton service
            //    if (typeInfo.GetInterfaces().Any(t => t == typeof(IComponentViewModel)))
            //        services.AddSingleton(type);
            //}

            //#endregion

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
