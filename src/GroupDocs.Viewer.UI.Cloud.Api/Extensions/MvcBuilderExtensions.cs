using System;
using System.Net.Http.Headers;
using System.Reflection;
using GroupDocs.Viewer.UI.Api.Controllers;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Handlers;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Cloud.Api.Viewers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddGroupDocsViewerCloudApi(this IMvcBuilder builder, 
            Action<Config> setupConfig = null)
        {
            var config = new Config();
            setupConfig?.Invoke(config);

            // GroupDocs.Viewer API Registration
            builder.PartManager.ApplicationParts.Add(new AssemblyPart(
                Assembly.GetAssembly(typeof(ViewerController))));

            builder.Services
                .AddOptions<Config>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.BindCloudApiSettings(settings);
                    setupConfig?.Invoke(settings);
                });

            //Register custom Bearer Token Handler. The DelegatingHandler has to be registered as a Transient Service
            builder.Services.AddTransient<ProtectedApiBearerTokenHandler>();

            //Register a Typed Instance of HttpClientFactory for a Protected Resource
            //More info see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1
            builder.Services.AddHttpClient<IViewerApiConnect, ViewerApiConnect>(client =>
            {
                client.BaseAddress = new Uri(config.ApiEndpoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<ProtectedApiBearerTokenHandler>();

            builder.Services.AddHttpClient<IAuthServerConnect, AuthServerConnect>(client =>
            {
                client.BaseAddress = new Uri(config.ApiEndpoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddTransient<IFileCache, NoopFileCache>();

            builder.Services.AddTransient<IViewer, PngViewer>();

            /*
            switch (config.ViewerType)
            {
                //case ViewerType.HtmlWithExternalResources:
                //    builder.Services.AddTransient<IViewer, HtmlWithExternalResourcesViewer>();
                //    break;
                //case ViewerType.Png:
                //    builder.Services.AddTransient<IViewer, PngViewer>();
                //    break;
                //case ViewerType.Jpg:
                //    builder.Services.AddTransient<IViewer, JpgViewer>();
                //    break;
                //default:
                //    builder.Services.AddTransient<IViewer, HtmlWithEmbeddedResourcesViewer>();
                //    break;
            }
            */
            return new GroupDocsViewerUIApiBuilder(builder.Services);
        }
    }
}
