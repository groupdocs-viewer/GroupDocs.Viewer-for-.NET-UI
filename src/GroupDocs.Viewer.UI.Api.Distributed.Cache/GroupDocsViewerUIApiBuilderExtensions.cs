using System;
using System.Linq;
using GroupDocs.Viewer.UI.Api.Distributed.Cache;
using GroupDocs.Viewer.UI.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DistributedCacheBuilderExtensions
    {
        /// <summary>
        /// Adds an IDistributedCache-based file cache.
        /// You must register an IDistributedCache implementation (e.g. AddStackExchangeRedisCache, AddDistributedSqlServerCache) separately.
        /// </summary>
        public static GroupDocsViewerUIApiBuilder AddDistributedCache(
            this GroupDocsViewerUIApiBuilder builder, Action<DistributedCacheEntryOptions> setupOptions = null)
        {
            var entryOptions = new DistributedCacheEntryOptions();
            setupOptions?.Invoke(entryOptions);

            builder.Services.AddSingleton(entryOptions);

            ServiceDescriptor registeredServiceDescriptor = builder.Services.FirstOrDefault(
                s => s.ServiceType == typeof(IFileCache));

            if (registeredServiceDescriptor != null)
            {
                builder.Services.Remove(registeredServiceDescriptor);
            }

            builder.Services.AddTransient<IFileCache, DistributedFileCache>();

            return builder;
        }
    }
}
