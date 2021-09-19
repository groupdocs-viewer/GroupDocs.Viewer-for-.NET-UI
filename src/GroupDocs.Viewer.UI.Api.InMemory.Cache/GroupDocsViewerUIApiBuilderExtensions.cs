using System;
using System.Linq;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Api.InMemory.Cache;
using GroupDocs.Viewer.UI.Api.InMemory.Cache.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GroupDocsViewerUIApiBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddInMemoryCache(
            this GroupDocsViewerUIApiBuilder builder, Action<Config> setupConfig = null)
        {
            var config = new Config();
            setupConfig?.Invoke(config);

            builder.Services
                .AddOptions<Config>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.BindInMemoryCacheSettings(settings);
                    setupConfig?.Invoke(settings);
                });

            builder.Services.TryAddSingleton<IMemoryCache, MemoryCache>();

            RegisterInMemoryFileCache(builder);

            return builder;
        }

        private static void RegisterInMemoryFileCache(GroupDocsViewerUIApiBuilder builder)
        {
            ServiceDescriptor registeredServiceDescriptor = builder.Services.FirstOrDefault(
                s => s.ServiceType == typeof(IFileCache));

            if (registeredServiceDescriptor != null)
            {
                builder.Services.Remove(registeredServiceDescriptor);
            }

            // NOTE: Replace is used here as by default we've registered Noop cache 
            builder.Services.AddTransient<IFileCache, InMemoryFileCache>();
        }
    }
}
