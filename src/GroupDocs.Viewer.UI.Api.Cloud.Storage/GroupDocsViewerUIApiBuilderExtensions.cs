using System;
using System.Net.Http.Headers;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Api.Cloud.Storage;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Handlers;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.Configuration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GroupDocsViewerUIApiBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddCloudStorage(
            this GroupDocsViewerUIApiBuilder builder, Action<Config> setupConfig)
        {
            if (setupConfig == null)
                throw new ArgumentNullException(nameof(setupConfig));

            var config = new Config();
            setupConfig.Invoke(config);

            //Add config options and bind configuration
            builder.Services
                .AddOptions<Config>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.BindCloudApiSettings(settings);
                    setupConfig.Invoke(settings);
                });

            //Register custom Bearer Token Handler. The DelegatingHandler has to be registered as a Transient Service
            builder.Services.AddTransient<ProtectedApiBearerTokenHandler>();

            //Register a Typed Instance of HttpClientFactory for a Protected Resource
            //More info see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1
            builder.Services.AddHttpClient<IStorageApiConnect, StorageApiConnect>(client =>
                {
                    client.BaseAddress = new Uri(config.ApiEndpoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .AddHttpMessageHandler<ProtectedApiBearerTokenHandler>();

            builder.Services.AddHttpClient<IAuthServerConnect, AuthServerConnect>(
                "StorageAuthServerConnect", client =>
            {
                client.BaseAddress = new Uri(config.ApiEndpoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddTransient<IFileStorage, CloudFileStorage>();

            return builder;
        }
    }
}
