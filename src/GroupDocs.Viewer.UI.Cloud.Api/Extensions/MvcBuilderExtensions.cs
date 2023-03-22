using System;
using System.Net.Http.Headers;
using System.Reflection;
using GroupDocs.Viewer.UI.Api;
using GroupDocs.Viewer.UI.Api.Controllers;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Handlers;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Cloud.Api.Viewers;
using GroupDocs.Viewer.UI.Core.Caching;
using GroupDocs.Viewer.UI.Core.Caching.Implementation;
using GroupDocs.Viewer.UI.Core.FileCaching;
using GroupDocs.Viewer.UI.Core.PageFormatting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddGroupDocsViewerCloudApi(this IMvcBuilder builder, 
            Action<Config> setupConfig = null)
        {
            var config = new Config();
            setupConfig?.Invoke(config);

            //Register ViewerController
            builder.PartManager.ApplicationParts.Add(new AssemblyPart(
                Assembly.GetAssembly(typeof(ViewerController))));

            //Add config options and bind configuration
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

            builder.Services.AddTransient<PngViewer>();
            builder.Services.AddTransient<JpgViewer>();
            builder.Services.AddTransient<HtmlWithEmbeddedResourcesViewer>();
            builder.Services.AddTransient<HtmlWithExternalResourcesViewer>();
            builder.Services.AddTransient<IAsyncLock, AsyncDuplicateLock>();
            builder.Services.AddTransient<IFileCache, NoopFileCache>();
            builder.Services.TryAddSingleton<IFileNameResolver, FilePathFileNameResolver>();
            builder.Services.TryAddSingleton<ISearchTermResolver, SearchTermResolver>();
            builder.Services.TryAddSingleton<IPageFormatter, NoopPageFormatter>();
            
            builder.Services.AddTransient<IViewer>(factory =>
            {
                IViewer viewer;
                switch (config.ViewerType)
                {
                    case ViewerType.HtmlWithExternalResources:
                        viewer = factory.GetRequiredService<HtmlWithExternalResourcesViewer>();
                        break;
                    case ViewerType.Png:
                        viewer = factory.GetRequiredService<PngViewer>();
                        break;
                    case ViewerType.Jpg:
                        viewer = factory.GetRequiredService<JpgViewer>();
                        break;
                    default:
                        viewer = factory.GetRequiredService<HtmlWithEmbeddedResourcesViewer>();
                        break;
                }

                return new CachingViewer(
                    viewer,
                    factory.GetRequiredService<IFileCache>(),
                    factory.GetRequiredService<IAsyncLock>()
                );
            });

            return new GroupDocsViewerUIApiBuilder(builder.Services);
        }
    }
}
